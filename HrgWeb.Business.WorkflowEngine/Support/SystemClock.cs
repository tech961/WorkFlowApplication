using System;

namespace HrgWeb.Business.WorkflowEngine.Support
{
    /// <summary>
    /// Default clock implementation that delegates to <see cref="DateTime.UtcNow"/>.
    /// </summary>
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
