namespace SimpleWorkflowEngine.EntityModels
{
    /// <summary>
    /// Describes the type of a workflow node.
    /// </summary>
    public enum ProcessNodeType
    {
        StartEvent,
        EndEvent,
        UserTask,
        ServiceTask,
        Timer
    }
}
