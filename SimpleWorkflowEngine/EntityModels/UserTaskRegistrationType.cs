namespace SimpleWorkflowEngine.EntityModels
{
    public class UserTaskRegistrationType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public int? Type { get; set; }
        public string TypeDesc { get; set; }
    }
}