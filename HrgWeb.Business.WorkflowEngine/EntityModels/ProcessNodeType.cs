namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    /// <summary>
    /// Describes the type of a workflow node.
    /// </summary>
    public enum ProcessNodeType
    {
        ServiceTaskNode,
        EndEvent,
        UserTask,
        ServiceTask,
        Timer
    }
}
