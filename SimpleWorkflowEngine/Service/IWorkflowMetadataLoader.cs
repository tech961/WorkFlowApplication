using System.Collections.Generic;
using SimpleWorkflowEngine.Models;

namespace SimpleWorkflowEngine.Service
{
    /// <summary>
    /// Allows additional metadata to be attached to node models after they are compiled.
    /// </summary>
    public interface IWorkflowMetadataLoader
    {
        void LoadMetadata(IEnumerable<ProcessNodeModel> nodes);
    }
}
