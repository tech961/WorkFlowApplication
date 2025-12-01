using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    /// <summary>
    /// Populates the execution context with metadata describing the node being executed.
    /// </summary>
    public sealed class ProcessNodeExecutionHandler : IProcessNodeExecutionHandler
    {
        private const string NodeIdKey = "CurrentNodeId";
        private const string NodeNameKey = "CurrentNodeName";
        private const string NodeKindKey = "CurrentNodeKind";

        public void Register(IExecutionContext context, ProcessNodeModel processNode)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (processNode == null)
            {
                throw new ArgumentNullException(nameof(processNode));
            }

            if (context.Items == null)
            {
                throw new InvalidOperationException("The execution context must provide a non-null Items dictionary.");
            }

            context.Items[NodeIdKey] = processNode.Id;
            context.Items[NodeNameKey] = processNode.Name;
            context.Items[NodeKindKey] = processNode.Kind;

            CopyMetadataToContext(context.Items, processNode.Metadata);
        }

        public void Execute(IExecutionContext context, ProcessNodeModel processNode)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (processNode == null)
            {
                throw new ArgumentNullException(nameof(processNode));
            }

            if (context.Items == null)
            {
                throw new InvalidOperationException("The execution context must provide a non-null Items dictionary.");
            }

            CopyMetadataToContext(context.Items, processNode.Metadata);
        }

        private static void CopyMetadataToContext(IDictionary<string, object> destination, IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object> item in metadata)
            {
                string key = string.Format("Node.{0}", item.Key);
                destination[key] = item.Value;
            }
        }
    }
}
