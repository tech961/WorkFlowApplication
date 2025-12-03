using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    /// <summary>
    /// Default execution context implementation used by the engine.
    /// </summary>
    public sealed class ExecutionContext : IInternalExecutionContext
    {
        public ExecutionContext()
        {
            Items = new Dictionary<string, object>();
            WorkflowDataList = new List<IWorkflowMetadata>();
            StepId = Guid.NewGuid();
        }

        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int FiscalYearId { get; set; }
        public IWorkflowVoucher Voucher { get; set; }

        public IEnumerable<IWorkflowMetadata> WorkflowDataList { get; set; }

        // اگر این اشتباهه و لازم نیست، حذفش کن
        public int WorkflowData { get; set; }

        public Guid StepId { get; set; }
        public bool SimulationMode { get; set; }

        public IDictionary<string, object> Items { get; }

        public IExecutionContext Initialize(int userId, int companyId, int fiscalYearId, IWorkflowVoucher workflowVoucher)
        {
            UserId = userId;
            CompanyId = companyId;
            FiscalYearId = fiscalYearId;
            Voucher = workflowVoucher ?? throw new ArgumentNullException(nameof(workflowVoucher));

            return this;
        }
    }
}
