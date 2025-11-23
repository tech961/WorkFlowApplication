using System;

namespace SimpleWorkflowEngine.Models
{
    /// <summary>
    /// Represents a compiled transition between two node models.
    /// </summary>
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
