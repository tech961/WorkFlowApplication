using System;

namespace SimpleWorkflowEngine.Support
{
    /// <summary>
    /// Abstraction over the system clock to ease testing.
    /// </summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
