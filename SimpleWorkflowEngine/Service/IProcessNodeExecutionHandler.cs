using SimpleWorkflowEngine.Models;
using SimpleWorkflowEngine.Runtime;

namespace SimpleWorkflowEngine.Service
{
    /// <summary>
    /// Receives notifications when the engine is about to execute a node.
    /// </summary>
    public interface IProcessNodeExecutionHandler
    {
        void Register(IExecutionContext context, ProcessNodeModel processNode);

        void Execute(IExecutionContext context, ProcessNodeModel processNode);
    }
}
