namespace SimpleWorkflowEngine.Runtime
{
    /// <summary>
    /// Represents the business document that triggered a workflow.
    /// </summary>
    public interface IWorkflowVoucher
    {
        int Id { get; }

        int Kind { get; }
    }
}
