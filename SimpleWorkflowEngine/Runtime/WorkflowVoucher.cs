namespace SimpleWorkflowEngine.Runtime
{
    /// <summary>
    /// Simple immutable voucher implementation.
    /// </summary>
    public sealed class WorkflowVoucher : IWorkflowVoucher
    {
        public WorkflowVoucher(int id, int kind)
        {
            Id = id;
            Kind = kind;
        }

        public int Id { get; }

        public int Kind { get; }
    }
}
