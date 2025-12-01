using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public class UserTaskNodeModel : ProcessNodeModel, IUserTaskNodeModel
    {
        private const string RegistrationTypeKey = "registrationType";
        private const string IsStartTaskKey = "isStartTask";

        public UserTaskNodeModel()
        {
        }

        public UserTaskNodeModel(ProcessNodeDefinition definition, IClock clock)
            : base(definition, clock)
        {
            RegistrationType = ReadRegistrationType(definition);
            IsStartTask = ReadIsStartTask(definition);
        }

        public UserTaskRegistrationType RegistrationType { get; set; }

        public bool IsStartTask { get; set; }

        public override NodeContinuation Continue(ProcessInstance instance, IInternalExecutionContext context, ProcessExecutionStep currentStep, IReadOnlyList<ProcessExecutionStep> previousSteps)
        {
            return new NodeContinuation
            {
                StepCompleted = false,
                WaitForExternalSignal = true,
                NextNode = null
            };
        }

        private static UserTaskRegistrationType ReadRegistrationType(ProcessNodeDefinition definition)
        {
            if (definition != null && definition.Settings.TryGetValue(RegistrationTypeKey, out string value))
            {
                if (Enum.TryParse(value, ignoreCase: true, out UserTaskRegistrationType registrationType))
                {
                    return registrationType;
                }
            }

            return UserTaskRegistrationType.None;
        }

        private static bool ReadIsStartTask(ProcessNodeDefinition definition)
        {
            if (definition != null && definition.Settings.TryGetValue(IsStartTaskKey, out string value))
            {
                return bool.TryParse(value, out bool isStart) && isStart;
            }

            return false;
        }
    }
}
