using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Support;
using HrgWeb.Business.WorkflowEngine.DataModel;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public class EndEventNodeModel : ProcessNodeModel, IEndEventNodeModel
    {
        public EndEventNodeModel()
        {
        }

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
