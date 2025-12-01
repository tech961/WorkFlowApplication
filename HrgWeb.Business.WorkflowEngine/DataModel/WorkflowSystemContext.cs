using System;
using System.Collections.Generic;
using System.Linq;

namespace HrgWeb.Business.WorkflowEngine.DataModel
{
    /// <summary>
    /// Simple in-memory store that keeps workflow definitions and runtime state.
    /// </summary>
    public sealed class WorkflowSystemContext
    {
        private readonly IList<ProcessDefinition> _processes = new List<ProcessDefinition>();
        private readonly IList<ProcessInstanceRecord> _instances = new List<ProcessInstanceRecord>();
        private readonly IList<ExecutionStepRecord> _executionSteps = new List<ExecutionStepRecord>();

        public IEnumerable<ProcessDefinition> Processes => _processes;

        public IEnumerable<ProcessInstanceRecord> ProcessInstances => _instances;

        public IEnumerable<ExecutionStepRecord> ExecutionSteps => _executionSteps;

        public void AddProcess(ProcessDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _processes.Add(definition);
        }

        public ProcessInstanceRecord CreateInstance(ProcessDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            int nextInstanceId = _instances.Any() ? _instances.Max(record => record.Id) + 1 : 1;
            var instance = new ProcessInstanceRecord(nextInstanceId, definition.Id, definition.VoucherKind);
            _instances.Add(instance);
            return instance;
        }

        public ProcessInstanceRecord GetInstance(int instanceId)
        {
            ProcessInstanceRecord instance = _instances.SingleOrDefault(record => record.Id == instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException($"Process instance '{instanceId}' was not found.");
            }

            return instance;
        }

        public ExecutionStepRecord AddStep(ExecutionStepRecord step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            _executionSteps.Add(step);
            return step;
        }

        public ExecutionStepRecord GetStep(Guid stepId)
        {
            ExecutionStepRecord step = _executionSteps.SingleOrDefault(record => record.Id == stepId);
            if (step == null)
            {
                throw new InvalidOperationException($"Execution step '{stepId}' was not found.");
            }

            return step;
        }

        public IEnumerable<ExecutionStepRecord> GetStepsForInstance(int instanceId)
        {
            return _executionSteps.Where(step => step.ProcessInstanceId == instanceId);
        }
    }
}
