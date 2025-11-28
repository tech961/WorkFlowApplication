using SimpleWorkflowEngine.Runtime;

namespace WorkFlowConsoleApp.Infrastructure
{
    public static class DependencyResolver
    {
        public static Container Container { get; } = new Container();

        public class Container
        {
            public T Resolve<T>() where T : class
            {
                if (typeof(T) == typeof(IExecutionContext))
                {
                    return new ExecutionContext(0, 0, 0, new WorkflowVoucher(0, 0)) as T;
                }

                return null;
            }
        }
    }
}

