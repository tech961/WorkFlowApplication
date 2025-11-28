namespace SimpleWorkflowEngine.EntityModels
{
    public class JoinPreviousProcessNode
    {
        public int ID { get; set; }
        public int JoinNodeID { get; set; }
        public int PreviousProcessNodeID { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsOptional { get; set; }
        public string OptionalCondition { get; set; }

        public ProcessNode PreviousProcessNode { get; set; }
    }
}