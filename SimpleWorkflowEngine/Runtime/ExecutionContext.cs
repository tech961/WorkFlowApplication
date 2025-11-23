using System;
using System.Collections.Generic;

namespace SimpleWorkflowEngine.Runtime
{
    /// <summary>
    /// Default execution context implementation used by the engine.
    /// </summary>
    public sealed class ExecutionContext : IInternalExecutionContext
    {
        public ExecutionContext(int userId, int companyId, int fiscalYearId, IWorkflowVoucher voucher)
        {
            UserId = userId;
            CompanyId = companyId;
            FiscalYearId = fiscalYearId;
            Voucher = voucher ?? throw new ArgumentNullException(nameof(voucher));
            Items = new Dictionary<string, object>();
            WorkflowData = new IWorkflowMetadata[0];
        }

        public int CompanyId { get; }

        public int UserId { get; }

        public int FiscalYearId { get; }

        public IWorkflowVoucher Voucher { get; }

        public IDictionary<string, object> Items { get; }

        public IEnumerable<IWorkflowMetadata> WorkflowData { get; set; }

        public Guid StepId { get; set; }

        public bool SimulationMode { get; set; }
    }
}
