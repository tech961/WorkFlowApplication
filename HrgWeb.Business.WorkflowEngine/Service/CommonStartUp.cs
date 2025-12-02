using Microsoft.Practices.Unity;
using HrgWeb.Business.WorkflowEngine.Runtime;
using HrgWeb.Business.WorkflowEngine.Support;

namespace HrgWeb.Business.WorkflowEngine.Service
{
    public class CommonStartUp
    {
        public static void OnStart()
        {
            DependencyResolver.Container.RegisterType<IClock, SystemClock>();
            DependencyResolver.Container.RegisterType<IWorkflowLogger, ConsoleWorkflowLogger>();

            DependencyResolver.Container.RegisterType<IExecutionContext, ExecutionContext>();
            DependencyResolver.Container.RegisterType<IInternalExecutionContext, ExecutionContext>();
            DependencyResolver.Container.RegisterType<IWorkflowVoucher, WorkflowVoucher>();
            DependencyResolver.Container.RegisterType<IServiceTaskNodeModel, ServiceTaskNodeModel>();
            DependencyResolver.Container.RegisterType<IUserTaskNodeModel, UserTaskNodeModel>();
            DependencyResolver.Container.RegisterType<IEndEventNodeModel, EndEventNodeModel>();
            DependencyResolver.Container.RegisterType<IWorkflowMetadata, WorkflowMetadata>();

            DependencyResolver.Container.RegisterType<IWorkflowVoucherLoader, WorkflowVoucherLoader>();
            DependencyResolver.Container.RegisterType<IWorkflowMetadataLoader, WorkflowMetadataLoader>();
            DependencyResolver.Container.RegisterType<IExpressionEvaluator, ExpressionEvaluator>();
            DependencyResolver.Container.RegisterType<IProcessNodeExecutionHandler, ProcessNodeExecutionHandler>();
            DependencyResolver.Container.RegisterType<IWorkflowService, WorkflowService>();
        }
    }
}
