using System;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    /// <summary>
    /// Abstraction that allows the engine to report runtime exceptions.
    /// </summary>
    public interface IWorkflowLogger
    {
        void LogError(Exception exception, string message);
    }
}
