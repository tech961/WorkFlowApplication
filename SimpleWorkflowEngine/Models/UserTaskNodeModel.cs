using System.Collections.Generic;
using SimpleWorkflowEngine.DataModel;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Support;

namespace SimpleWorkflowEngine.Models
{
    public sealed class UserTaskNodeModel : ProcessNodeModel
    {
        private const string RegistrationTypeKey = "registrationType";

        public UserTaskNodeModel(ProcessNodeDefinition definition, IClock clock)
            : base(definition, clock)
        {
            RegistrationType = definition.Settings.TryGetValue(RegistrationTypeKey, out string value)
                ? value
                : string.Empty;
        }

        public string RegistrationType { get; }

        public override NodeContinuation Continue(ProcessInstanceRecord instance, IInternalExecutionContext context, ExecutionStepRecord currentStep, IReadOnlyList<ExecutionStepRecord> previousSteps)
        {
            // User tasks wait for external completion.
            return new NodeContinuation
            {
                StepCompleted = false,
                WaitForExternalSignal = true,
                NextNode = null
            };
        }
    }
}
