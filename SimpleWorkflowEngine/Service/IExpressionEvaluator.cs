using System;
using SimpleWorkflowEngine.Runtime;

namespace SimpleWorkflowEngine.Service
{
    /// <summary>
    /// Provides expression evaluation services to the workflow engine.
    /// </summary>
    public interface IExpressionEvaluator
    {
        bool EvaluateCondition(IExecutionContext context, string expression);

        DateTime EvaluateDateTime(IExecutionContext context, string expression);
    }
}
