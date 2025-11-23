using System;
using System.Collections.Generic;
using System.Linq;
using SimpleWorkflowEngine.DataModel;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Support;

namespace SimpleWorkflowEngine.Models
{
    /// <summary>
    /// Base type for runtime workflow nodes.
    /// </summary>
    public abstract class ProcessNodeModel
    {
        protected ProcessNodeModel(ProcessNodeDefinition definition, IClock clock)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Transitions = new List<NodeTransition>();
            PreviousNodes = new List<ProcessNodeModel>();
            Metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        protected ProcessNodeDefinition Definition { get; }

        protected IClock Clock { get; }

        public int Id => Definition.Id;

        public string Name => Definition.Name;

        public ProcessNodeKind Kind => Definition.Kind;

        public ProcessModel Process { get; internal set; }

        public IDictionary<string, string> Settings
        {
            get { return Definition.Settings; }
        }

        public IDictionary<string, object> Metadata { get; }

        public IList<NodeTransition> Transitions { get; }

        public IList<ProcessNodeModel> PreviousNodes { get; }

        public virtual ExecutionStepRecord CreateStep(ProcessInstanceRecord instance, IExecutionContext context)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var step = new ExecutionStepRecord
            {
                ProcessInstanceId = instance.Id,
                NodeId = Id,
                CreatedOnUtc = Clock.UtcNow
            };

            return step;
        }

        public abstract NodeContinuation Continue(ProcessInstanceRecord instance, IInternalExecutionContext context, ExecutionStepRecord currentStep, IReadOnlyList<ExecutionStepRecord> previousSteps);

        public ProcessNodeModel GetNext(string condition = null)
        {
            if (Transitions.Count == 0)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(condition))
            {
                return Transitions[0].Target;
            }

            NodeTransition matching = Transitions.FirstOrDefault(transition => string.Equals(transition.Condition, condition, StringComparison.OrdinalIgnoreCase));
            return matching?.Target;
        }
    }
}
