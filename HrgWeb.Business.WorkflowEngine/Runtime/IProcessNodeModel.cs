using HrgWeb.Business.WorkflowEngine.DataModel;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public interface IProcessNodeModel : IWorkflowMetadataSupport
    {
        int ID { get; set; }
        string Name { get; set; }
        ProcessNodeKind NodeKind { get; set; }
    }
}
