namespace SimpleWorkflowEngine.EntityModels
{
    public class ProcessNodeKind
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ProcessNodeType? Type { get; set; }
        public string TypeDesc { get; set; }
        public string Remark { get; set; }
    }
}