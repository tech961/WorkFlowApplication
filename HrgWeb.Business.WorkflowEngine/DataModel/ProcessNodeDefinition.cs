using HrgWeb.Business.WorkflowEngine.EntityModels;
using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.DataModel
{
    /// <summary>
    /// Raw node definition as it is stored in configuration.
    /// </summary>
    public sealed class ProcessNodeDefinition
    {
        public ProcessNodeDefinition(int id, string name, ProcessNodeKind kind)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Kind = kind;
        }

        public int Id { get; }

        public string Name { get; }

        public ProcessNodeKind Kind { get; }

        public IList<TransitionDefinition> Transitions { get; } = new List<TransitionDefinition>();

        public IDictionary<string, string> Settings { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ProcessNodeDefinition AddTransition(int targetNodeId, string condition = null)
        {
            Transitions.Add(new TransitionDefinition(targetNodeId, condition));
            return this;
        }

        public ProcessNodeDefinition WithSetting(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("A setting key must be provided.", nameof(key));
            }

            Settings[key] = value ?? string.Empty;
            return this;
        }
    }
}
