using System;

namespace HrgWeb.Business.WorkflowEngine.DataModel
{
    /// <summary>
    /// Represents the definition of an outgoing transition from a workflow node.
    /// </summary>
    public sealed class TransitionDefinition
    {
        public TransitionDefinition(int targetNodeId, string condition = null)
        {
            if (targetNodeId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetNodeId));
            }

            TargetNodeId = targetNodeId;
            Condition = condition;
        }

        public int TargetNodeId { get; }

        public string Condition { get; }
    }
}
