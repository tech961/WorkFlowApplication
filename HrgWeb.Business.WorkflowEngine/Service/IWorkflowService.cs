using System;
using System.Collections.Generic;
using HrgWeb.Business.WorkflowEngine.Models;
using HrgWeb.Business.WorkflowEngine.Runtime;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    public interface IWorkflowService
    {
        void InitializeService(string engineConnectionString);

        void RegisterLoader(IWorkflowMetadataLoader metadataLoader);

        void RegisterProcessNodeExecutionHandler(IProcessNodeExecutionHandler executionHandler);

        void RegisterExpressionEvaluator(IExpressionEvaluator expressionEvaluatorInstance);

        void RegisterWorkflowVoucherLoader(IWorkflowVoucherLoader workflowVoucherLoaderInstance);

        void InitializeEngine();

        void StartProcessInstance(int voucherKind, IExecutionContext executionContext);

        IEnumerable<UserTaskNodeModel> GetNextUserTasks(IExecutionContext executionContext);

        void ExecuteUserTask(IExecutionContext executionContext);

        void ProcessTimers(DateTime? now = null);
    }
}
