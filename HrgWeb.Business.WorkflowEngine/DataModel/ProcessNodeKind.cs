namespace HrgWeb.Business.WorkflowEngine.DataModel
{
    public enum ProcessNodeKind
    {
        None = 0,
        StartEventNode = 1,
        UserTaskNode = 2,
        ServiceTaskNode = 3,
        ForkNode = 4,
        JoinNode = 5,
        EndEventNode = 6,
        Timer = 7
    }
}
