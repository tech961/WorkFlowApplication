namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public class WorkflowMetadata : IWorkflowMetadata
    {
        public WorkflowMetadata(int id)
        {
            ID = id;
        }

        public int ID { get; set; }
    }
}
