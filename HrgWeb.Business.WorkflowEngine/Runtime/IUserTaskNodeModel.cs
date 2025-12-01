using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.EntityModels;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public interface IUserTaskNodeModel : IProcessNodeModel, IWorkflowMetadataSupport
    {
        UserTaskRegistrationType RegistrationType { get; set; }
        bool IsStartTask { get; set; }
    }
}
