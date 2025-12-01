using System;

namespace HrgWeb.Business.WorkflowEngine.Support
{
    /// <summary>
    /// Abstraction over the system clock to ease testing.
    /// </summary>
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
