using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    /// <summary>
    /// Context passed to the workflow engine when executing process nodes.
    /// </summary>
    public interface IExecutionContext
    {
        int CompanyId { get; }

        int UserId { get; }

        int FiscalYearId { get; }

        IWorkflowVoucher Voucher { get; }

        IEnumerable<IWorkflowMetadata> WorkflowData { get; set; }

        Guid StepId { get; set; }

        IExecutionContext Initialize(int userId, int companyId, int fiscalYearId, IWorkflowVoucher workflowVoucher);
    }
}
