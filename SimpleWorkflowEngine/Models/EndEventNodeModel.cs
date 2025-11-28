using System.Collections.Generic;
using SimpleWorkflowEngine.EntityModels;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Support;

namespace SimpleWorkflowEngine.Models
{
    public sealed class EndEventNodeModel : ProcessNodeModel
    {
        public EndEventNodeModel(ProcessNode definition, IClock clock)
            : base(definition, clock)
        {
        }

        public override NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps)
        {
            currentStep.IsCompleted = true;
            currentStep.CompletedOnUtc = Clock.UtcNow;
            instance.IsClosed = true;
            return new NodeContinuation
            {
                StepCompleted = true,
                NextNode = null
            };
        }
    }
}
