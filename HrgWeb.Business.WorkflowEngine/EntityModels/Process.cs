using HrgWeb.Business.WorkflowEngine.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class Process
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int VoucherKindID { get; set; }
        public int Version { get; set; }
        public bool Active { get; set; } = true;
        public int RegUserID { get; set; }
        public DateTime RegDate { get; set; }
        public int? ModifyUserID { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int RegCompanyID { get; set; }
        public int VoucherKind { get; set; }

        public IList<ProcessNode> Nodes { get; set; } = new List<ProcessNode>();

        public ProcessNode GetNode(int nodeId)
        {
            ProcessNode definition = Nodes.SingleOrDefault(node => node.ID == nodeId);
            if (definition == null)
            {
                throw new InvalidOperationException($"Node {nodeId} was not found in process '{Name}'.");
            }

            return definition;
        }

        public ProcessNode GetStartNode()
        {
            ProcessNode startNode = Nodes.SingleOrDefault(node => node.NodeKind == ProcessNodeKind.StartEventNode);
            if (startNode == null)
            {
                throw new InvalidOperationException($"Process '{Name}' does not contain a start event node.");
            }

            return startNode;
        }
    }
}
