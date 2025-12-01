using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Runtime;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Models
{
    public sealed class UserTaskNodeModel : ProcessNodeModel
    {
        private const string RegistrationTypeKey = "registrationType";
        private const string IsStartTaskKey = "isStartTask";

        public UserTaskNodeModel(ProcessNode definition, IClock clock)
            : base(definition, clock)
        {
            RegistrationType = ParseRegistrationType(definition.Settings);
            IsStartTask = ParseIsStartTask(definition.Settings);
        }

        public UserTaskRegistrationType RegistrationType { get; }

        public bool IsStartTask { get; }

        public override NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps)
        {
            // User tasks wait for external completion.
            return new NodeContinuation
            {
                StepCompleted = false,
                WaitForExternalSignal = true,
                NextNode = null
            };
        }

        private static UserTaskRegistrationType ParseRegistrationType(IDictionary<string, string> settings)
        {
            if (settings.TryGetValue(RegistrationTypeKey, out string value)
                && Enum.TryParse(value, ignoreCase: true, out UserTaskRegistrationType registrationType))
            {
                return registrationType;
            }

            return UserTaskRegistrationType.None;
        }

        private static bool ParseIsStartTask(IDictionary<string, string> settings)
        {
            return settings.TryGetValue(IsStartTaskKey, out string value)
                && bool.TryParse(value, out bool isStart)
                && isStart;
        }
    }
}
