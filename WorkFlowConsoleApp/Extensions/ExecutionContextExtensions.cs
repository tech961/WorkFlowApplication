//using System;
//using SimpleWorkflowEngine.Runtime;

//namespace WorkFlowConsoleApp.Extensions
//{
//    public static class ExecutionContextExtensions
//    {
//        public static IExecutionContext Initialize(this IExecutionContext context, int userId, int companyId, int fiscalYearId, IWorkflowVoucher voucher)
//        {
//            if (voucher == null)
//            {
//                throw new ArgumentNullException(nameof(voucher), "Voucher cannot be null");
//            }

//            var newContext = new ExecutionContext(userId, companyId, fiscalYearId, voucher);

//            if (context != null)
//            {
//                newContext.WorkflowData = context.WorkflowData;
//                newContext.StepId = context.StepId;

//                if (context is ExecutionContext existingContext)
//                {
//                    newContext.SimulationMode = existingContext.SimulationMode;
//                }
//            }

//            return newContext;
//        }
//    }
//}

