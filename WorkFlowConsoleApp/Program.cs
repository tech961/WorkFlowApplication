using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using SimpleWorkflowEngine.DataModel;
using SimpleWorkflowEngine.Models;
using SimpleWorkflowEngine.Runtime;
using SimpleWorkflowEngine.Service;
using WorkFlowConsoleApp.DataAccess;
using WorkFlowConsoleApp.Extensions;
using WorkFlowConsoleApp.Infrastructure;

namespace WorkFlowConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SimpleWorkflow Engine Test ===");
            Console.WriteLine();

            WorkflowService workflowService = new WorkflowService();

            string connectionString = ConfigurationManager.ConnectionStrings["WorkflowEngine"]?.ConnectionString 
                ?? "data source=192.168.2.5\web;initial catalog=ConvertMokatebat;user id=afshar;password=12345678q@;multipleactiveresultsets=True;;";
            
            workflowService.InitializeService(connectionString);
            Console.WriteLine($" Connection String تنظیم شد");
            Console.WriteLine();

            Console.WriteLine("=== تست Dapper ===");
            DapperRepository repository = new DapperRepository(connectionString);
            
            bool connectionTest = repository.TestConnection();
            if (connectionTest)
            {
                Console.WriteLine(" اتصال به دیتابیس موفق بود");
                
                int count = repository.GetProcessInstanceCount();
                Console.WriteLine($" تعداد Process Instances در دیتابیس: {count}");
                
                var instances = repository.GetProcessInstances();
                if (instances.Any())
                {
                    Console.WriteLine($" {instances.Count()} Process Instance از دیتابیس خوانده شد");
                    foreach (var instance in instances.Take(5))
                    {
                        Console.WriteLine($"  - ID: {instance.Id}, Process ID: {instance.ProcessId}, IsClosed: {instance.IsClosed}");
                    }
                }
                else
                {
                    Console.WriteLine(" هیچ Process Instance در دیتابیس یافت نشد");
                }
            }
            else
            {
                Console.WriteLine("اتصال به دیتابیس موفق نبود (احتمالاً جدول وجود ندارد یا دیتابیس در دسترس نیست)");
            }
            Console.WriteLine();

            ProcessDefinition processDefinition = CreateSampleProcess();
            workflowService.Context.AddProcess(processDefinition);

            workflowService.RegisterProcessNodeExecutionHandler(new DefaultProcessNodeExecutionHandler());
            workflowService.RegisterExpressionEvaluator(new DefaultExpressionEvaluator());
            workflowService.RegisterLoader(new DefaultWorkflowMetadataLoader());

            workflowService.InitializeEngine();

            Console.WriteLine($" Process '{processDefinition.Name}' اضافه شد");
            Console.WriteLine($"  - Process ID: {processDefinition.Id}");
            Console.WriteLine($"  - Voucher Kind: {processDefinition.VoucherKind}");
            Console.WriteLine($"  - Nodes: {processDefinition.Nodes.Count}");
            Console.WriteLine();

            int voucherId = 1001;
            int voucherKind = 1;
            IWorkflowVoucher voucher = new WorkflowVoucher(voucherId, voucherKind);

            int userId = 1;
            int companyId = 101;
            int fiscalYearId = 2024;

            IExecutionContext context = new ExecutionContext(userId, companyId, fiscalYearId, voucher);

            Console.WriteLine(" Execution Context ایجاد شد:");
            Console.WriteLine($"  - User ID: {userId}");
            Console.WriteLine($"  - Company ID: {companyId}");
            Console.WriteLine($"  - Fiscal Year ID: {fiscalYearId}");
            Console.WriteLine($"  - Voucher ID: {voucher.Id}");
            Console.WriteLine($"  - Voucher Kind: {voucher.Kind}");
            Console.WriteLine();

            try
            {
                Console.WriteLine("شروع Process Instance...");
                
                StartProcess(workflowService, voucherKind, userId, companyId, fiscalYearId, voucher);
                
                Console.WriteLine("✓ Process Instance با موفقیت شروع شد");
                Console.WriteLine();

                var executionContext = DependencyResolver.Container.Resolve<IExecutionContext>().Initialize(
                    userId, companyId, fiscalYearId, voucher);
                
                IEnumerable<UserTaskNodeModel> userTasks = workflowService.GetNextUserTasks(executionContext);
                var tasksList = userTasks.ToList();

                if (tasksList.Any())
                {
                    Console.WriteLine($"✓ {tasksList.Count} User Task پیدا شد:");
                    foreach (var task in tasksList)
                    {
                        Console.WriteLine($"  - Task ID: {task.Id}");
                        Console.WriteLine($"  - Task Name: {task.Name}");
                        Console.WriteLine($"  - Step ID: {task.StepId}");
                        Console.WriteLine();
                    }

                    if (tasksList.Count > 0)
                    {
                        var firstTask = tasksList[0];
                        executionContext.StepId = firstTask.StepId;

                        Console.WriteLine($"اجرای User Task '{firstTask.Name}'...");
                        workflowService.ExecuteUserTask(executionContext);
                        Console.WriteLine("✓ User Task با موفقیت اجرا شد");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine(" هیچ User Task در حال انتظار پیدا نشد");
                }

                Console.WriteLine("Process Instances:");
                foreach (var instance in workflowService.Context.ProcessInstances)
                {
                    string status = instance.IsClosed ? "بسته شده" : "فعال";
                    Console.WriteLine($"  - Instance ID: {instance.Id}, Process ID: {instance.ProcessId}, Status: {status}");
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("=== پایان ===");
            Console.WriteLine("برای خروج یک کلید بزنید...");
            Console.ReadKey();
        }

        static ProcessDefinition CreateSampleProcess()
        {
            ProcessDefinition process = new ProcessDefinition(
                id: 1,
                name: "فرآیند تایید نمونه",
                voucherKind: 1,
                version: 1,
                isActive: true
            );

            ProcessNodeDefinition startNode = new ProcessNodeDefinition(
                id: 10,
                name: "شروع",
                kind: ProcessNodeKind.StartEvent
            );
            startNode.AddTransition(targetNodeId: 20); 
            process.Nodes.Add(startNode);

            ProcessNodeDefinition userTaskNode = new ProcessNodeDefinition(
                id: 20,
                name: "تایید توسط کاربر",
                kind: ProcessNodeKind.UserTask
            );
            userTaskNode.AddTransition(targetNodeId: 30);
            process.Nodes.Add(userTaskNode);

            ProcessNodeDefinition endNode = new ProcessNodeDefinition(
                id: 30,
                name: "پایان",
                kind: ProcessNodeKind.EndEvent
            );
            process.Nodes.Add(endNode);

            return process;
        }

        static void StartProcess(IWorkflowService workflowService, int voucherKind, int userId, int companyId, int fiscalYearId, IWorkflowVoucher voucher)
        {
            var executionContext = DependencyResolver.Container.Resolve<IExecutionContext>().Initialize(
                userId,
                companyId,
                fiscalYearId,
                voucher);

            workflowService.StartProcessInstance(voucherKind, executionContext);
        }
    }
}   
