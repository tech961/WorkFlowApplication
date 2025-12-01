using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWorkflowEngine.EntityModels
{
    /// <summary>
    /// In-memory representation of workflow definitions and runtime state using EntityModel types.
    /// </summary>
    public sealed class WorkflowSystemContext
    {
        private readonly IList<Process> _processes = new List<Process>();
        private readonly IList<ProcessInstance> _instances = new List<ProcessInstance>();
        private readonly IList<ProcessExecutionStep> _executionSteps = new List<ProcessExecutionStep>();

        public IEnumerable<Process> Processes => _processes;

        public IEnumerable<ProcessInstance> ProcessInstances => _instances;

        public IEnumerable<ProcessExecutionStep> ExecutionSteps => _executionSteps;

        public void AddProcess(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            _processes.Add(process);
        }

        public ProcessInstance CreateInstance(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            int nextInstanceId = _instances.Any() ? _instances.Max(record => record.ID) + 1 : 1;
            var instance = new ProcessInstance
            {
                ID = nextInstanceId,
                ProcessID = process.ID,
                VoucherKindID = process.VoucherKindID
            };

            _instances.Add(instance);
            return instance;
        }

        public ProcessInstance GetInstance(int instanceId)
        {
            ProcessInstance instance = _instances.SingleOrDefault(record => record.ID == instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException($"Process instance '{instanceId}' was not found.");
            }

            return instance;
        }

        public ProcessExecutionStep AddStep(ProcessExecutionStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            _executionSteps.Add(step);
            return step;
        }

        public ProcessExecutionStep GetStep(Guid stepId)
        {
            ProcessExecutionStep step = _executionSteps.SingleOrDefault(record => record.ID == stepId);
            if (step == null)
            {
                throw new InvalidOperationException($"Execution step '{stepId}' was not found.");
            }

            return step;
        }

        public IEnumerable<ProcessExecutionStep> GetStepsForInstance(int instanceId)
        {
            return _executionSteps.Where(step => step.ProcessInstanceId == instanceId);
        }
    }
}
