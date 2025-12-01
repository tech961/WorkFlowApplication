using System;

namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public sealed class NodeTransition
    {
        public NodeTransition(ProcessNodeModel target, string condition)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Condition = condition;
        }

        public ProcessNodeModel Target { get; }

        public string Condition { get; }
    }
}
