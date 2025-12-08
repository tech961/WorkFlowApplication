namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class ServiceTaskNode
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public string Metadata { get; set; }

        public ProcessNode ProcessNode { get; set; }
    }
}