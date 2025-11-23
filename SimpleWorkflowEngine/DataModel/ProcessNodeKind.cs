namespace SimpleWorkflowEngine.DataModel
{
    /// <summary>
    /// Describes the type of a workflow node.
    /// </summary>
    public enum ProcessNodeKind
    {
        StartEvent,
        EndEvent,
        UserTask,
        ServiceTask,
        Timer
    }
}
