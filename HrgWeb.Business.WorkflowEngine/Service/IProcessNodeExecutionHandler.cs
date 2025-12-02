using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// Receives notifications when the engine is about to execute a node.
    /// </summary>
    public interface IProcessNodeExecutionHandler
    {
        void Register(IExecutionContext context, IProcessNodeModel processNode);

        void Execute(IExecutionContext context, IProcessNodeModel processNode);
    }
}
