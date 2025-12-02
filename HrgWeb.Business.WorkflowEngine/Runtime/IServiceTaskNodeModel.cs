namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public interface IServiceTaskNodeModel : IProcessNodeModel
    {
        int TypeId { get; set; }

        string MetaData { get; set; }
    }
}
