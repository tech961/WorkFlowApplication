namespace SimpleWorkflowEngine.Models
{
    /// <summary>
    /// Result returned after a node has executed and the engine needs to decide what to do next.
    /// </summary>
    public sealed class NodeContinuation
    {
        public ProcessNodeModel NextNode { get; set; }

        public bool StepCompleted { get; set; }

        public bool WaitForExternalSignal { get; set; }
    }
}
