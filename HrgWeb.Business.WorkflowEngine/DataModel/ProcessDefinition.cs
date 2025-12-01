//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace HrgWeb.Business.WorkflowEngine.DataModel
//{
//    /// <summary>
//    /// Describes a workflow process consisting of nodes and transitions.
//    /// </summary>
//    public sealed class ProcessDefinition
//    {
//        public ProcessDefinition(int id, string name, int voucherKind, int version, bool isActive)
//        {
//            if (id <= 0)
//            {
//                throw new ArgumentOutOfRangeException(nameof(id));
//            }

//            if (version < 0)
//            {
//                throw new ArgumentOutOfRangeException(nameof(version));
//            }

//            Id = id;
//            Name = name ?? throw new ArgumentNullException(nameof(name));
//            VoucherKind = voucherKind;
//            Version = version;
//            IsActive = isActive;
//        }

//        public int Id { get; }

//        public string Name { get; }

//        public int VoucherKind { get; }

//        public int Version { get; }

//        public bool IsActive { get; }

//        public IList<ProcessNodeDefinition> Nodes { get; } = new List<ProcessNodeDefinition>();

//        public ProcessNodeDefinition GetNode(int nodeId)
//        {
//            ProcessNodeDefinition definition = Nodes.SingleOrDefault(node => node.Id == nodeId);
//            if (definition == null)
//            {
//                throw new InvalidOperationException($"Node {nodeId} was not found in process '{Name}'.");
//            }

//            return definition;
//        }

//        public ProcessNodeDefinition GetStartNode()
//        {
//            ProcessNodeDefinition startNode = Nodes.SingleOrDefault(node => node.Kind == ProcessNodeKind.StartEvent);
//            if (startNode == null)
//            {
//                throw new InvalidOperationException($"Process '{Name}' does not contain a start event node.");
//            }

//            return startNode;
//        }
//    }
//}
