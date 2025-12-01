namespace HrgWeb.Business.WorkflowEngine.DataModel
{
    /// <summary>
    /// Describes the type of a workflow node.
    /// </summary>
    public enum ProcessNodeKind
    {
        StartEventNode,
        UserTaskNode,
        ServiceTaskNode,
        ForkNode,
        JoinNode,
        EndEventNode,
        Timer
    }
}
