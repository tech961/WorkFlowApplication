using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// Loads vouchers associated with workflow instances.
    /// </summary>
    public interface IWorkflowVoucherLoader
    {
        IWorkflowVoucher GetWorkflowVoucher(int voucherId, int voucherKind);
    }
}
