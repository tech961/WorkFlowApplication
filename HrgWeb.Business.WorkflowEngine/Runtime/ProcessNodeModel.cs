using System;
using System.Collections.Generic;
using System.Linq;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public abstract class ProcessNodeModel : IProcessNodeModel
    {
        protected ProcessNodeModel()
            : this(definition: null, clock: new SystemClock())
        {
        }

        protected ProcessNodeModel(ProcessNode definition, IClock clock)
        {
            Clock = clock ?? throw new ArgumentNullException(nameof(clock));
            Transitions = new List<NodeTransition>();
            PreviousNodes = new List<ProcessNodeModel>();
            Metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (definition != null)
            {
                ID = definition.ID;
                Name = definition.Name;
                NodeKind = definition.NodeKind;
                foreach (KeyValuePair<string, string> setting in definition.Settings)
                {
                    Settings[setting.Key] = setting.Value;
                }
            }
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public ProcessNodeKind NodeKind { get; set; }

        public IWorkflowMetadata MetadataModel { get; set; }

        protected IClock Clock { get; }

        public IDictionary<string, string> Settings { get; }

        public IDictionary<string, object> Metadata { get; }

        public IList<NodeTransition> Transitions { get; }

        public IList<ProcessNodeModel> PreviousNodes { get; }

        public ProcessModel Process { get; set; }

        public virtual ProcessExecutionStep CreateStep(ProcessInstance instance, IExecutionContext context)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var step = new ProcessExecutionStep
            {
                ID = Guid.NewGuid(),
                ProcessInstanceID = instance.ID,
                ProcessID = instance.ProcessID,
                ProcessNodeID = ID,
                CreatedOnUtc = Clock.UtcNow,
                PathID = Guid.NewGuid(),
                RegisterDateTime = Clock.UtcNow.Ticks,
                RegDate = Clock.UtcNow
            };

            return step;
        }

        public abstract NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps);

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
