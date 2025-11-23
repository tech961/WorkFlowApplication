using SimpleWorkflowEngine.Runtime;

namespace SimpleWorkflowEngine.Service
{
    /// <summary>
    /// Loads vouchers associated with workflow instances.
    /// </summary>
    public interface IWorkflowVoucherLoader
    {
        IWorkflowVoucher GetWorkflowVoucher(int voucherId, int voucherKind);
    }
}
