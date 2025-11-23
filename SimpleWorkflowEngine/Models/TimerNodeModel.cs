using System;
using System.Collections.Generic;
using SimpleWorkflowEngine.DataModel;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Support;

namespace SimpleWorkflowEngine.Models
{
    public sealed class TimerNodeModel : ProcessNodeModel
    {
        private const string DelayExpressionKey = "delay";

        public TimerNodeModel(ProcessNodeDefinition definition, IClock clock)
            : base(definition, clock)
        {
            DelayExpression = definition.Settings.TryGetValue(DelayExpressionKey, out string value)
                ? value
                : string.Empty;
        }

        public string DelayExpression { get; }

        public override ExecutionStepRecord CreateStep(ProcessInstanceRecord instance, IExecutionContext context)
        {
            ExecutionStepRecord step = base.CreateStep(instance, context);
            step.Metadata["DelayExpression"] = DelayExpression;
            return step;
        }

        public override NodeContinuation Continue(ProcessInstanceRecord instance, IInternalExecutionContext context, ExecutionStepRecord currentStep, IReadOnlyList<ExecutionStepRecord> previousSteps)
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
