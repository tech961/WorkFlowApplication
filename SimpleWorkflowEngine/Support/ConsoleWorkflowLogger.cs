using System;
using SimpleWorkflowEngine.Runtime;

namespace SimpleWorkflowEngine.Support
{
    /// <summary>
    /// Basic logger that writes errors to the console output.
    /// </summary>
    public sealed class ConsoleWorkflowLogger : IWorkflowLogger
    {
        public void LogError(Exception exception, string message)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Console.Error.WriteLine($"[Workflow] {message}\n{exception}");
        }
    }
}
