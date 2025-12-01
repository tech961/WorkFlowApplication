namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class ForkNextProcessNode
    {
        public int ID { get; set; }
        public int ForkNodeID { get; set; }
        public int NextProcessNodeID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Condition { get; set; }
        public string DesignerLinkPath { get; set; }

        public ForkNode ForkNode { get; set; }
        public ProcessNode NextProcessNode { get; set; }
    }
}