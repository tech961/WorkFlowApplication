using System;
using System.Collections.Generic;

namespace SimpleWorkflowEngine.EntityModels
{
    /// <summary>
    /// Stores runtime information for a workflow instance using database-aligned fields.
    /// </summary>
    public class ProcessInstance
    {
        public int ID { get; set; }

        public int Id
        {
            get => ID;
            set => ID = value;
        }

        public int ProcessID { get; set; }

        public int ProcessId
        {
            get => ProcessID;
            set => ProcessID = value;
        }

        public int VoucherKindID { get; set; }

        public bool IsClosed { get; set; }

        public IDictionary<string, object> Data { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
