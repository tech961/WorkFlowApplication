using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.Models;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// Allows additional metadata to be attached to node models after they are compiled.
    /// </summary>
    public interface IWorkflowMetadataLoader
    {
        void LoadMetadata(IEnumerable<ProcessNodeModel> nodes);
    }
}
