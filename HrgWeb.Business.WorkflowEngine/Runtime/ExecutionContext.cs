using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.Runtime
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

        public int CompanyId { get; private set; }

        public int UserId { get; private set; }

        public int FiscalYearId { get; private set; }

        public IWorkflowVoucher Voucher { get; private set; }

        public IDictionary<string, object> Items { get; private set; }

        public IEnumerable<IWorkflowMetadata> WorkflowData { get; set; }

        public Guid StepId { get; set; }

        public bool SimulationMode { get; set; }

        public IExecutionContext Initialize(int userId, int companyId, int fiscalYearId, IWorkflowVoucher workflowVoucher)
        {
            UserId = userId;
            CompanyId = companyId;
            FiscalYearId = fiscalYearId;
            Voucher = workflowVoucher;

            return this;
        }
    }
}
