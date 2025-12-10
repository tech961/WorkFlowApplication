using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.Engine;
using HrgWeb.Business.WorkflowEngine.Runtime;
using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// High level facade that orchestrates the execution engine and exposes a convenient API.
    /// </summary>
    public sealed class WorkflowService : IWorkflowService
    {
        private readonly WorkflowSystemContext _context;
        private readonly ExecutionEngine _engine;
        private readonly IClock _clock;
        private IWorkflowVoucherLoader _voucherLoader;
        private string _connectionString;

        public WorkflowService()
            : this(new WorkflowSystemContext(), new SystemClock(), new ConsoleWorkflowLogger())
        {
        }

        public WorkflowService(string connectionString)
            : this(new WorkflowSystemContext(), new SystemClock(), new ConsoleWorkflowLogger())
        {
            _connectionString = connectionString;
        }

        public WorkflowService(WorkflowSystemContext context, IClock clock, IWorkflowLogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _engine = new ExecutionEngine(_context, clock);
            _engine.RegisterLogger(logger);
        }

        public void InitializeService(string engineConnectionString)
        {
            _connectionString = engineConnectionString;
        }

        public void RegisterLoader(IWorkflowMetadataLoader metadataLoader)
        {
            _engine.RegisterMetadataLoader(metadataLoader);
        }

        public void RegisterProcessNodeExecutionHandler(IProcessNodeExecutionHandler executionHandler)
        {
            _engine.RegisterExecutionHandler(executionHandler);
        }

        public void RegisterExpressionEvaluator(IExpressionEvaluator expressionEvaluatorInstance)
        {
            _engine.RegisterExpressionEvaluator(expressionEvaluatorInstance);
        }

        public void RegisterWorkflowVoucherLoader(IWorkflowVoucherLoader workflowVoucherLoaderInstance)
        {
            _voucherLoader = workflowVoucherLoaderInstance ?? throw new ArgumentNullException(nameof(workflowVoucherLoaderInstance));
        }

        public void InitializeEngine()
        {
            _engine.LoadProcessDefinitions(_context.Processes);
        }

        public void StartProcessInstance(int voucherKind, IExecutionContext executionContext)
        {
            IInternalExecutionContext internalContext = EnsureInternalContext(executionContext);
            if (internalContext.Voucher == null)
            {
                throw new InvalidOperationException("The execution context must provide a workflow voucher.");
            }

            if (_voucherLoader != null)
            {
                IWorkflowVoucher voucher = _voucherLoader.GetWorkflowVoucher(internalContext.Voucher.ID, voucherKind);
                internalContext = new ExecutionContext()
                {
                    UserId = internalContext.UserId,
                    WorkflowData = internalContext.WorkflowData,
                    StepId = internalContext.StepId,
                    SimulationMode = internalContext.SimulationMode,
                    CompanyId = internalContext.CompanyId,
                    FiscalYearId = internalContext.FiscalYearId,
                    Voucher = voucher
                };
            }

            //internalContext.Voucher.Kind = voucherKind;
            _engine.StartProcessInstance(voucherKind, internalContext);
        }


        public IEnumerable<UserTaskNodeModel> GetNextUserTasks(IExecutionContext executionContext)
        {
            if (executionContext == null)
            {
                throw new ArgumentNullException(nameof(executionContext));
            }

            IInternalExecutionContext internalContext = EnsureInternalContext(executionContext);
            if (internalContext.Voucher == null)
            {
                throw new InvalidOperationException("The execution context must provide a workflow voucher.");
            }

            if (_voucherLoader != null)
            {
                //IWorkflowVoucher voucher = _voucherLoader.GetWorkflowVoucher(internalContext.Voucher.ID, internalContext.Voucher.Kind);
                //internalContext = new ExecutionContext()
                //{
                //    UserId = internalContext.UserId,
                //    WorkflowData = internalContext.WorkflowData,
                //    WorkflowDataList = internalContext.WorkflowDataList,
                //    StepId = internalContext.StepId,
                //    SimulationMode = internalContext.SimulationMode,
                //    CompanyId = internalContext.CompanyId,
                //    FiscalYearId = internalContext.FiscalYearId,
                //    Voucher = voucher
                //};
            }

            return _engine.GetPendingUserTasks(internalContext);
        }

        public void ExecuteUserTask(IExecutionContext executionContext)
        {
            if (executionContext == null)
            {
                throw new ArgumentNullException(nameof(executionContext));
            }

            if (executionContext.StepId == Guid.Empty)
            {
                throw new InvalidOperationException("The execution context must contain the identifier of the step to execute.");
            }

            IInternalExecutionContext internalContext = EnsureInternalContext(executionContext);
            _engine.CompleteUserTask(executionContext.StepId, internalContext);
        }

        public void ProcessTimers(DateTime? now = null)
        {
            _engine.ProcessDueTimers(now ?? _clock.UtcNow);
        }

        public WorkflowSystemContext Context => _context;

        public string ConnectionString => _connectionString;

        private IInternalExecutionContext EnsureInternalContext(IExecutionContext executionContext)
        {
            if (executionContext is IInternalExecutionContext internalContext)
            {
                return internalContext;
            }

            return new ExecutionContext()
            {
                UserId = executionContext.UserId,
                CompanyId = executionContext.CompanyId,
                FiscalYearId = executionContext.FiscalYearId,
                Voucher = executionContext.Voucher,
                WorkflowData = executionContext.WorkflowData,
                WorkflowDataList = executionContext.WorkflowDataList,
                StepId = executionContext.StepId
            };
        }

        // The registered voucher loader is retained for future extensions.
    }
}
