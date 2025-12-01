using System.Collections.Generic;
using SimpleWorkflowEngine.EntityModels;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Support;

namespace SimpleWorkflowEngine.Models
{
    public sealed class ServiceTaskNodeModel : ProcessNodeModel
    {
        public ServiceTaskNodeModel(ProcessNode definition, IClock clock)
            : base(definition, clock)
        {
        }

        public override NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps)
        {
            currentStep.IsCompleted = true;
            currentStep.CompletedOnUtc = Clock.UtcNow;
            return new NodeContinuation
            {
                StepCompleted = true,
                NextNode = GetNext()
            };
        }
    }
}
