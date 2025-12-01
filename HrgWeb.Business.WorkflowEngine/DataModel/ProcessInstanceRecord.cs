using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.DataModel
{
    /// <summary>
    /// Stores runtime information for a workflow instance.
    /// </summary>
    public sealed class ProcessInstanceRecord
    {
        public ProcessInstanceRecord(int id, int processId, int voucherKind)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            if (processId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(processId));
            }

            Id = id;
            ProcessId = processId;
            VoucherKind = voucherKind;
        }

        public int Id { get; }

        public int ProcessId { get; }

        public int VoucherKind { get; }

        public bool IsClosed { get; set; }

        public IDictionary<string, object> Data { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
