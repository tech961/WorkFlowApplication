namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public sealed class NodeContinuation
    {
        public ProcessNodeModel NextNode { get; set; }

        public bool StepCompleted { get; set; }

        public bool WaitForExternalSignal { get; set; }
    }
}
