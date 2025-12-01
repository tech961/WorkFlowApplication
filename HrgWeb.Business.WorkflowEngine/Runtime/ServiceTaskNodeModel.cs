using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public class ServiceTaskNodeModel : ProcessNodeModel
    {
        public ServiceTaskNodeModel()
        {
        }

        public ServiceTaskNodeModel(ProcessNodeDefinition definition, IClock clock)
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
