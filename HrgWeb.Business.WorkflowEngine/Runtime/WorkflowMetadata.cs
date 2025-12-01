namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public class WorkflowMetadata : IWorkflowMetadata
    {
        public WorkflowMetadata(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }
    }
}
