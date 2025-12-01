using HrgWeb.Business.WorkflowEngine.DataModel;
using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class ProcessNode
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ProcessID { get; set; }
        public int NodeKindID { get; set; }
        public int? NextProcessNodeID { get; set; }
        public string DesignerLocation { get; set; }
        public string DesignerLinkPath { get; set; }
        public int RegUserID { get; set; }
        public DateTime RegDate { get; set; }
        public int? ModifyUserID { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int RegCompanyID { get; set; }

        public Process Process { get; set; }
        public ProcessNode NextProcessNode { get; set; }
        public ProcessNodeKind NodeKind { get; set; }

        public ProcessNodeType NodeType => (ProcessNodeType)NodeKindID;

        public IList<ForkNextProcessNode> ForkNextProcessNodes { get; set; } = new List<ForkNextProcessNode>();

        public IDictionary<string, string> Settings { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
