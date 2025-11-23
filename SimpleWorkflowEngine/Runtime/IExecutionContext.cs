using System;
using System.Collections.Generic;

namespace SimpleWorkflowEngine.Runtime
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

        IDictionary<string, object> Items { get; }

        IEnumerable<IWorkflowMetadata> WorkflowData { get; set; }

        Guid StepId { get; set; }
    }
}
