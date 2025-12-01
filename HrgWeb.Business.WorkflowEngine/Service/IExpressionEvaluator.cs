using System;
using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
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
