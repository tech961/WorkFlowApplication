using System;

namespace SimpleWorkflowEngine.Support
{
    /// <summary>
    /// Default clock implementation that delegates to <see cref="DateTime.UtcNow"/>.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
