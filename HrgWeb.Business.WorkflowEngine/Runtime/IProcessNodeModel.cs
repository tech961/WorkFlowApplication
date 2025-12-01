using HrgWeb.Business.WorkflowEngine.DataModel;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public interface IProcessNodeModel : IWorkflowMetadataSupport
    {
        string Name { get; set; }
        ProcessNodeKind NodeKind { get; set; }
    }
}
