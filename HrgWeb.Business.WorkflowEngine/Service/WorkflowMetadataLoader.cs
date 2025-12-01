using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// Applies metadata to compiled node models using configurable dictionaries.
    /// </summary>
    public sealed class WorkflowMetadataLoader : IWorkflowMetadataLoader
    {
        private readonly IDictionary<int, IDictionary<string, object>> _metadataByNodeId;
        private readonly IDictionary<string, IDictionary<string, object>> _metadataByNodeName;

        public WorkflowMetadataLoader()
            : this(new Dictionary<int, IDictionary<string, object>>(), new Dictionary<string, IDictionary<string, object>>(StringComparer.OrdinalIgnoreCase))
        {
        }

        public WorkflowMetadataLoader(
            IDictionary<int, IDictionary<string, object>> metadataByNodeId,
            IDictionary<string, IDictionary<string, object>> metadataByNodeName)
        {
            _metadataByNodeId = metadataByNodeId ?? new Dictionary<int, IDictionary<string, object>>();
            _metadataByNodeName = metadataByNodeName ?? new Dictionary<string, IDictionary<string, object>>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddMetadata(int nodeId, string key, object value)
        {
            if (!_metadataByNodeId.TryGetValue(nodeId, out IDictionary<string, object> metadata))
            {
                metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                _metadataByNodeId[nodeId] = metadata;
            }

            metadata[key] = value;
        }

        public void AddMetadata(string nodeName, string key, object value)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                throw new ArgumentException("A node name must be provided.", nameof(nodeName));
            }

            IDictionary<string, object> metadata;
            if (!_metadataByNodeName.TryGetValue(nodeName, out metadata))
            {
                metadata = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                _metadataByNodeName[nodeName] = metadata;
            }

            metadata[key] = value;
        }

        public void LoadMetadata(IEnumerable<ProcessNodeModel> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            foreach (ProcessNodeModel node in nodes)
            {
                IDictionary<string, object> metadata;
                if (_metadataByNodeId.TryGetValue(node.ID, out metadata))
                {
                    ApplyMetadata(node, metadata);
                }

                if (_metadataByNodeName.TryGetValue(node.Name, out metadata))
                {
                    ApplyMetadata(node, metadata);
                }
            }
        }

        private static void ApplyMetadata(ProcessNodeModel node, IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> item in metadata)
            {
                node.Metadata[item.Key] = item.Value;
            }
        }
    }
}
