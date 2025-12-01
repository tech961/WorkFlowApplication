namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class UserTaskNode
    {
        public int ID { get; set; }
        public int RegistrationTypeID { get; set; }
        public bool IsStartTask { get; set; }

        public ProcessNode ProcessNode { get; set; }
        public UserTaskRegistrationType RegistrationType { get; set; }
    }
}