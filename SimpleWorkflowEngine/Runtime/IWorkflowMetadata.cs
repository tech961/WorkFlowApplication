namespace SimpleWorkflowEngine.Runtime
{
    /// <summary>
    /// Metadata item attached to a workflow execution.
    /// </summary>
    public interface IWorkflowMetadata
    {
        string Key { get; }

        object Value { get; }
    }
}
