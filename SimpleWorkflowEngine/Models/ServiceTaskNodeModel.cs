using System.Collections.Generic;
using SimpleWorkflowEngine.DataModel;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Support;

namespace SimpleWorkflowEngine.Models
{
    public sealed class ServiceTaskNodeModel : ProcessNodeModel
    {
        public ServiceTaskNodeModel(ProcessNodeDefinition definition, IClock clock)
            : base(definition, clock)
        {
        }

        public override NodeContinuation Continue(ProcessInstanceRecord instance, IInternalExecutionContext context, ExecutionStepRecord currentStep, IReadOnlyList<ExecutionStepRecord> previousSteps)
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
