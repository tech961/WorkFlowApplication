//using System;
//using System.Collections.Generic;
//using System.Linq;
//using HrgWeb.Business.WorkflowEngine.EntityModels;
//using HrgWeb.Business.WorkflowEngine.Runtime;
//using HrgWeb.Business.WorkflowEngine.Support;

//namespace HrgWeb.Business.WorkflowEngine.Models
//{
//    /// <summary>
//    /// Base type for runtime workflow nodes.
//    /// </summary>
//    public abstract class ProcessNodeModel
//    {
//        protected ProcessNodeModel(ProcessNode definition, IClock clock)
//        {
//            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
//            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
//            Transitions = new List<NodeTransition>();
//            PreviousNodes = new List<ProcessNodeModel>();
//            Metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
//        }

//        protected ProcessNode Definition { get; }

//        protected IClock Clock { get; }

//        public int ID => Definition.ID;

//        public string Name => Definition.Name;

//        public ProcessNodeType Kind => Definition.NodeType;

//        public ProcessModel Process { get; internal set; }

//        public IDictionary<string, string> Settings
//        {
//            get { return Definition.Settings; }
//        }

//        public IDictionary<string, object> Metadata { get; }

//        public IList<NodeTransition> Transitions { get; }

//        public IList<ProcessNodeModel> PreviousNodes { get; }

//        public virtual ProcessExecutionStep CreateStep(ProcessInstance instance, IExecutionContext context)
//        {
//            if (instance == null)
//            {
//                throw new ArgumentNullException(nameof(instance));
//            }

//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            var step = new ProcessExecutionStep
//            {
//                ID = Guid.NewGuid(),
//                ProcessInstanceId = instance.ID,
//                ProcessID = instance.ProcessID,
//                ProcessNodeID = ID,
//                CreatedOnUtc = Clock.UtcNow,
//                PathID = Guid.NewGuid(),
//                RegisterDateTime = Clock.UtcNow.Ticks,
//                RegDate = Clock.UtcNow
//            };

//            return step;
//        }

//        public abstract NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps);

//        public ProcessNodeModel GetNext(string condition = null)
//        {
//            if (Transitions.Count == 0)
//            {
//                return null;
//            }

//            if (string.IsNullOrWhiteSpace(condition))
//            {
//                return Transitions[0].Target;
//            }

//            NodeTransition matching = Transitions.FirstOrDefault(transition => string.Equals(transition.Condition, condition, StringComparison.OrdinalIgnoreCase));
//            return matching?.Target;
//        }
//    }
//}
