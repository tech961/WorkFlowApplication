namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    /// <summary>
    /// Extends the public execution context with engine-specific flags.
    /// </summary>
    public interface IInternalExecutionContext : IExecutionContext
    {
        bool SimulationMode { get; }
    }
}
