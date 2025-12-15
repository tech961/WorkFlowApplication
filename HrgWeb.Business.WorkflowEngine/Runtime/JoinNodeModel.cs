using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public class JoinNodeModel : ProcessNodeModel
    {
        public JoinNodeModel()
        {
        }

        public JoinNodeModel(ProcessNode definition, IClock clock)
            : base(definition, clock)
        {
        }

        public override NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps)
        {
            currentStep.Done = true;
            currentStep.CompletedOnUtc = Clock.UtcNow;

            return new NodeContinuation
            {
                StepCompleted = true,
                NextNode = GetNext()
            };
        }
    }
}