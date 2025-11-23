using System;
using System.Collections.Generic;

namespace SimpleWorkflowEngine.DataModel
{
    /// <summary>
    /// Represents an execution step for a process node.
    /// </summary>
    public sealed class ExecutionStepRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PathId { get; set; } = Guid.NewGuid();

        public int ProcessInstanceId { get; set; }

        public int NodeId { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? CompletedOnUtc { get; set; }

        public IList<Guid> PreviousStepIds { get; } = new List<Guid>();

        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    }
}
