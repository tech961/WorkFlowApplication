using System;
using System.Collections.Generic;
using System.Linq;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.Runtime;
using HrgWeb.Business.WorkflowEngine.Service;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Engine
{
    /// <summary>
    /// Core workflow execution engine that drives process instances.
    /// </summary>
    internal sealed class ExecutionEngine
    {
        private readonly WorkflowSystemContext _context;
        private readonly IClock _clock;
        private readonly TimerScheduler _scheduler;
        private readonly IDictionary<int, List<ProcessModel>> _processesByVoucherKind;
        private readonly IDictionary<int, ProcessModel> _processesById;
        private static readonly IList<IWorkflowMetadataLoader> _metadataLoaders = new List<IWorkflowMetadataLoader>();
        private static readonly IList<IProcessNodeExecutionHandler> _executionHandlers = new List<IProcessNodeExecutionHandler>();
        private IExpressionEvaluator _expressionEvaluator;
        private IWorkflowLogger _logger;

        public ExecutionEngine(WorkflowSystemContext context, IClock clock)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _scheduler = new TimerScheduler(clock);
            _processesByVoucherKind = new Dictionary<int, List<ProcessModel>>();
            _processesById = new Dictionary<int, ProcessModel>();
        }

        public void RegisterMetadataLoader(IWorkflowMetadataLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            _metadataLoaders.Add(loader);
        }

        public void RegisterExecutionHandler(IProcessNodeExecutionHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _executionHandlers.Add(handler);
        }

        public void RegisterExpressionEvaluator(IExpressionEvaluator evaluator)
        {
            _expressionEvaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

        public void RegisterLogger(IWorkflowLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void LoadProcessDefinitions(IEnumerable<Process> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            _processesByVoucherKind.Clear();
            _processesById.Clear();

            foreach (Process definition in definitions)
            {
                ProcessModel model = CompileProcess(definition);
                if (!_processesByVoucherKind.TryGetValue(definition.VoucherKindID, out List<ProcessModel> list))
                {
                    list = new List<ProcessModel>();
                    _processesByVoucherKind.Add(definition.VoucherKindID, list);
                }

                list.RemoveAll(existing => existing.Definition.Version == definition.Version);
                list.Add(model);
                list.Sort((left, right) => left.Definition.Version.CompareTo(right.Definition.Version));
                _processesById[definition.ID] = model;

                foreach (IWorkflowMetadataLoader loader in _metadataLoaders)
                {
                    loader.LoadMetadata(model.Nodes);
                }
            }
        }

        public void LoadProcessDefinitions(int voucherKind, IInternalExecutionContext context)
        {
            var activeProcess = _context.ActiveProcess(voucherKind, context);

            _processesByVoucherKind.Clear();
            _processesById.Clear();

            ProcessModel model = CompileProcess(activeProcess);
            if (!_processesByVoucherKind.TryGetValue(activeProcess.VoucherKindID, out List<ProcessModel> list))
            {
                list = new List<ProcessModel>();
                _processesByVoucherKind.Add(activeProcess.VoucherKindID, list);
            }

            list.RemoveAll(existing => existing.Definition.Version == activeProcess.Version);
            list.Add(model);
            list.Sort((left, right) => left.Definition.Version.CompareTo(right.Definition.Version));
            _processesById[activeProcess.ID] = model;

            foreach (IWorkflowMetadataLoader loader in _metadataLoaders)
            {
                loader.LoadMetadata(model.Nodes);
            }
        }

        public void StartProcessInstance(int voucherKind, IInternalExecutionContext context)
        {
            LoadProcessDefinitions(voucherKind, context);
            ProcessModel process = GetActiveProcessModel(voucherKind, context);
            ProcessInstance instance = _context.CreateInstance(process.Definition, context);
            ExecuteNode(process, instance, process.StartNode, context, previousStep: null);
        }

        public IEnumerable<UserTaskNodeModel> GetPendingUserTasks(int voucherId)
        {
            var processInstance = _context.ProcessInstances
                .FirstOrDefault(x => x.VoucherID == voucherId);

            if (processInstance == null)
                return Enumerable.Empty<UserTaskNodeModel>();

            var steps = _context.ExecutionSteps
                .Where(x => x.ProcessID == processInstance.ID && !x.Done)
                .ToList();

            var result = new List<UserTaskNodeModel>();

            foreach (var step in steps)
            {
                var processNode = _context.ProcessNode(step.ProcessNodeID);

                result.Add(new UserTaskNodeModel(processNode, _clock));


                foreach (IWorkflowMetadataLoader loader in _metadataLoaders)
                {
                    loader.LoadMetadata(processNode);
                }

            }


            return result;
        }

        public void CompleteUserTask(Guid stepId, IInternalExecutionContext context)
        {
            ProcessExecutionStep step = _context.GetStep(stepId);
            ProcessInstance instance = _context.GetInstance(step.ProcessInstanceID);
            ProcessModel process = _processesById[instance.ProcessID];
            ProcessNodeModel node = process.GetNode(step.ProcessNodeID);
            if (!(node is UserTaskNodeModel))
            {
                throw new InvalidOperationException("The specified step does not belong to a user task node.");
            }

            FinalizeWaitingNode(process, instance, node, step, context);
        }

        public void ProcessDueTimers(DateTime? now = null)
        {
            DateTime timestamp = now ?? _clock.UtcNow;
            foreach (TimerRequest request in _scheduler.CollectDueTimers(timestamp))
            {
                try
                {
                    ResumeTimer(request);
                }
                catch (Exception exception)
                {
                    _logger?.LogError(exception, $"Failed to resume timer for step {request.StepId}.");
                }
            }
        }

        private ProcessModel CompileProcess(Process definition)
        {
            var nodeLookup = new Dictionary<int, ProcessNodeModel>();
            foreach (ProcessNode nodeDefinition in definition.Nodes)
            {
                nodeLookup[nodeDefinition.ID] = CreateNodeModel(nodeDefinition);
            }

            foreach (ProcessNode nodeDefinition in definition.Nodes)
            {
                ProcessNodeModel source = nodeLookup[nodeDefinition.ID];

                if (nodeDefinition.ForkNextProcessNodes != null && nodeDefinition.ForkNextProcessNodes.Any())
                {
                    foreach (ForkNextProcessNode transition in nodeDefinition.ForkNextProcessNodes)
                    {
                        ProcessNodeModel target = nodeLookup[transition.NextProcessNodeID];
                        source.Transitions.Add(new NodeTransition(target, transition.Condition));
                        target.PreviousNodes.Add(source);
                    }
                }
                else if (nodeDefinition.NextProcessNodeID.HasValue)
                {
                    ProcessNodeModel target = nodeLookup[nodeDefinition.NextProcessNodeID.Value];
                    source.Transitions.Add(new NodeTransition(target, condition: null));
                    target.PreviousNodes.Add(source);
                }
            }

            return new ProcessModel(definition, nodeLookup.Values);
        }

        private ProcessNodeModel CreateNodeModel(ProcessNode definition)
        {
            switch (definition.NodeKind)
            {
                case ProcessNodeKind.StartEventNode:
                    return new StartEventNodeModel(definition, _clock);
                case ProcessNodeKind.EndEventNode:
                    return new EndEventNodeModel(definition, _clock);
                case ProcessNodeKind.UserTaskNode:
                    return new UserTaskNodeModel(definition, _clock);
                case ProcessNodeKind.ServiceTaskNode:
                    return new ServiceTaskNodeModel(definition, _clock);
                case ProcessNodeKind.Timer:
                    return new TimerNodeModel(definition, _clock);
                default:
                    throw new InvalidOperationException(string.Format("Unsupported node kind '{0}'.", definition.NodeKind));
            }
        }

        private ProcessModel GetActiveProcessModel(int voucherKind, IInternalExecutionContext context)
        {
            if (!_processesByVoucherKind.TryGetValue(voucherKind, out List<ProcessModel> list) || list.Count == 0)
            {
                throw new InvalidOperationException($"No process definitions were loaded for voucher kind {voucherKind}.");
            }

            ProcessModel activeProcess = list.LastOrDefault(model => model.Definition.Active);
            if (activeProcess == null)
            {
                throw new InvalidOperationException($"No active process definition was found for voucher kind {voucherKind}.");
            }

            return activeProcess;
        }

        private void ExecuteNode(ProcessModel process, ProcessInstance instance, ProcessNodeModel node, IInternalExecutionContext context, ProcessExecutionStep previousStep = null)
        {
            foreach (IProcessNodeExecutionHandler handler in _executionHandlers)
            {
                handler.Register(context, node);
            }

            ProcessExecutionStep step = node.CreateStep(instance, context);
            if (previousStep != null)
            {
                step.PreviousStepIds.Add(previousStep.ID);
                step.PathID = previousStep.PathID;
            }

            step.CreatedOnUtc = _clock.UtcNow;
            _context.AddStep(step);
            context.StepId = step.ID;

            //foreach (IProcessNodeExecutionHandler handler in _executionHandlers)
            //{
            //    handler.Execute(context, node);
            //}

            NodeContinuation continuation = node.Continue(instance, context, step, new ProcessExecutionStep[0]);
            if (continuation.StepCompleted && !step.Done)
            {
                step.Done = true;
                step.CompletedOnUtc = _clock.UtcNow;
            }

            ProcessNodeModel nextNode = continuation.NextNode ?? ResolveNextNode(node, context);

            _context.UpdateStep(step);

            if (continuation.WaitForExternalSignal)
            {
                if (node is TimerNodeModel timerNode)
                {
                    ScheduleTimer(instance, timerNode, step, context);
                }

                return;
            }

            if (nextNode != null)
            {
                ExecuteNode(process, instance, nextNode, context, step);
            }
        }

        private void ScheduleTimer(ProcessInstance instance, TimerNodeModel timerNode, ProcessExecutionStep step, IInternalExecutionContext context)
        {
            DateTime dueDate = timerNode.EvaluateDueDate(context);
            var request = new TimerRequest
            {
                StepId = step.ID,
                ProcessInstanceId = instance.ID,
                NodeId = timerNode.ID,
                ExecuteAtUtc = dueDate,
                UserId = context.UserId,
                CompanyId = context.CompanyId,
                FiscalYearId = context.FiscalYearId,
                VoucherId = context.Voucher.ID,
                WorkflowDataList = context.WorkflowDataList != null
                    ? context.WorkflowDataList.Select(item => new WorkflowMetadata(item.ID)).ToArray()
                    : new WorkflowMetadata[0]
            };
            _scheduler.Enqueue(request);
        }

        private void ResumeTimer(TimerRequest request)
        {
            ProcessExecutionStep step = _context.GetStep(request.StepId);
            ProcessInstance instance = _context.GetInstance(request.ProcessInstanceId);
            ProcessModel process = _processesById[instance.ProcessID];
            ProcessNodeModel node = process.GetNode(request.NodeId);
            if (!(node is TimerNodeModel))
            {
                throw new InvalidOperationException("Timer scheduler attempted to resume a non-timer node.");
            }

            var voucher = new WorkflowVoucher(request.VoucherId, "1", "", request.VoucherKind);
            var resumeContext = new ExecutionContext()
            {
                UserId = request.UserId,
                CompanyId = request.CompanyId,
                FiscalYearId = request.FiscalYearId,
                Voucher = voucher,
                WorkflowData = request.WorkflowData,
                WorkflowDataList = request.WorkflowDataList,
                StepId = request.StepId
            };

            FinalizeWaitingNode(process, instance, node, step, resumeContext);
        }

        private void FinalizeWaitingNode(ProcessModel process, ProcessInstance instance, ProcessNodeModel node, ProcessExecutionStep step, IInternalExecutionContext context)
        {
            step.Done = true;
            step.CompletedOnUtc = _clock.UtcNow;
            ProcessNodeModel nextNode = ResolveNextNode(node, context);
            _context.UpdateStep(step);
            if (nextNode != null)
            {
                ExecuteNode(process, instance, nextNode, context, step);
            }
        }

        private ProcessNodeModel ResolveNextNode(ProcessNodeModel node, IInternalExecutionContext context)
        {
            if (node.Transitions.Count == 0)
            {
                return null;
            }

            if (node.Transitions.Count == 1)
            {
                return node.Transitions[0].Target;
            }

            foreach (NodeTransition transition in node.Transitions)
            {
                if (string.IsNullOrWhiteSpace(transition.Condition))
                {
                    continue;
                }

                if (_expressionEvaluator != null && _expressionEvaluator.EvaluateCondition(context, transition.Condition))
                {
                    return transition.Target;
                }
            }

            NodeTransition defaultTransition = node.Transitions.FirstOrDefault(transition => string.IsNullOrWhiteSpace(transition.Condition));
            return defaultTransition?.Target;
        }

        private sealed class TimerScheduler
        {
            private readonly SortedSet<TimerRequest> _scheduledTimers;

            public TimerScheduler(IClock clock)
            {
                _scheduledTimers = new SortedSet<TimerRequest>(TimerRequestComparer.Instance);
            }

            public void Enqueue(TimerRequest request)
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                _scheduledTimers.Add(request);
            }

            public IReadOnlyList<TimerRequest> CollectDueTimers(DateTime timestamp)
            {
                var ready = new List<TimerRequest>();
                while (_scheduledTimers.Count > 0)
                {
                    TimerRequest next = _scheduledTimers.Min;
                    if (next.ExecuteAtUtc > timestamp)
                    {
                        break;
                    }

                    _scheduledTimers.Remove(next);
                    ready.Add(next);
                }

                return ready;
            }
        }

        private sealed class TimerRequestComparer : IComparer<TimerRequest>
        {
            public static readonly TimerRequestComparer Instance = new TimerRequestComparer();

            public int Compare(TimerRequest x, TimerRequest y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (x is null)
                {
                    return -1;
                }

                if (y is null)
                {
                    return 1;
                }

                int result = x.ExecuteAtUtc.CompareTo(y.ExecuteAtUtc);
                if (result != 0)
                {
                    return result;
                }

                return x.StepId.CompareTo(y.StepId);
            }
        }

        private sealed class TimerRequest
        {
            public Guid StepId { get; set; }

            public int ProcessInstanceId { get; set; }

            public int NodeId { get; set; }

            public DateTime ExecuteAtUtc { get; set; }

            public int UserId { get; set; }

            public int CompanyId { get; set; }

            public int FiscalYearId { get; set; }

            public int VoucherId { get; set; }

            public int VoucherKind { get; set; }

            public int WorkflowData { get; set; }
            public IReadOnlyList<WorkflowMetadata> WorkflowDataList { get; set; } = new WorkflowMetadata[0];
        }
        private void EnsureProcessDefinitionsLoaded()
        {
            if (_processesByVoucherKind.Count == 0 || _processesById.Count == 0)
            {
                LoadProcessDefinitions(_context.Processes);
            }
        }

    }
}
