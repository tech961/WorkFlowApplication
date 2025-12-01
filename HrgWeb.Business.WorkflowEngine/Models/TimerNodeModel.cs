using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Runtime;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Models
{
    public sealed class TimerNodeModel : ProcessNodeModel
    {
        private const string DelayExpressionKey = "delay";

        public TimerNodeModel(ProcessNode definition, IClock clock)
            : base(definition, clock)
        {
            DelayExpression = definition.Settings.TryGetValue(DelayExpressionKey, out string value)
                ? value
                : string.Empty;
        }

        public string DelayExpression { get; }

        public override ProcessExecutionStep CreateStep(ProcessInstance instance, IExecutionContext context)
        {
            ProcessExecutionStep step = base.CreateStep(instance, context);
            step.Metadata["DelayExpression"] = DelayExpression;
            return step;
        }

        public override NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps)
        {
            DateTime dueDate = EvaluateDueDate(context);
            currentStep.Metadata["DueDateUtc"] = dueDate;
            return new NodeContinuation
            {
                StepCompleted = false,
                WaitForExternalSignal = true,
                NextNode = null
            };
        }

        public DateTime EvaluateDueDate(IExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(DelayExpression))
            {
                return Clock.UtcNow;
            }

            if (DateTime.TryParse(DelayExpression, out DateTime absoluteDate))
            {
                return absoluteDate.ToUniversalTime();
            }

            if (TimeSpan.TryParse(DelayExpression, out TimeSpan delay))
            {
                return Clock.UtcNow.Add(delay);
            }

            throw new InvalidOperationException($"Timer delay expression '{DelayExpression}' is not a valid DateTime or TimeSpan value.");
        }
    }
}
