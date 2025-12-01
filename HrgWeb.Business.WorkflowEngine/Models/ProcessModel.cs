using System;
using System.Collections.Generic;
using System.Linq;
using HrgWeb.Business.WorkflowEngine.EntityModels;

namespace HrgWeb.Business.WorkflowEngine.Models
{
    /// <summary>
    /// Compiled workflow model used at runtime.
    /// </summary>
    public sealed class ProcessModel
    {
        private readonly Dictionary<int, ProcessNodeModel> _nodesById;

        public ProcessModel(Process definition, IEnumerable<ProcessNodeModel> nodes)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            var compiledNodes = nodes.ToList();
            foreach (ProcessNodeModel node in compiledNodes)
            {
                node.Process = this;
            }

            _nodesById = compiledNodes.ToDictionary(node => node.Id);
            Nodes = compiledNodes;
            StartNode = compiledNodes.OfType<StartEventNodeModel>().Single();
        }

        public Process Definition { get; }

        public IEnumerable<ProcessNodeModel> Nodes { get; }

        public StartEventNodeModel StartNode { get; }

        public ProcessNodeModel GetNode(int id)
        {
            if (!_nodesById.TryGetValue(id, out ProcessNodeModel node))
            {
                throw new InvalidOperationException($"Node {id} was not found in process '{Definition.Name}'.");
            }

            return node;
        }
    }
}
