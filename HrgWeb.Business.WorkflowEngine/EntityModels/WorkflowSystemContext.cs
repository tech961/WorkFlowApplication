using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using HrgWeb.Business.Workflow;
using HrgWeb.Business.WorkflowEngine.Context;
using HrgWeb.Business.WorkflowEngine.DataModel;
using HrgWeb.Business.WorkflowEngine.Runtime;
using HrgWeb.Common.FW;

namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public sealed class WorkflowSystemContext
    {
        private readonly HrgWorkFlowDb _dbContext;
        private readonly IList<Process> _processes;

        public WorkflowSystemContext()
        {

            _dbContext = BussinessSetting.BusinessContext;
            //_processes = LoadProcessesFromDatabase();
        }

        public IEnumerable<Process> Processes => _processes;

        public IEnumerable<ProcessInstance> ProcessInstances => _dbContext.WF_Process_m.ToList().Select(MapProcessInstance);

        public IEnumerable<ProcessExecutionStep> ExecutionSteps => _dbContext.WF_ProcessExecutionStep_m.ToList().Select(MapExecutionStep);

        public void AddProcess(Process process)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            _processes.Add(process);
        }

        public ProcessInstance CreateInstance(Process process, IInternalExecutionContext context)
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var instanceEntity = new WF_Process_m
            {
                ProcessMetaDataID = process.ID,
                VoucherRowID = context.Voucher.ID,
                Closed = false,
                RegUserID = process.RegUserID,
                RegDate = ServerTime.PersianNow().ToStandardDateTime(),
                RegCompanyID = process.RegCompanyID
            };

            _dbContext.WF_Process_m.Add(instanceEntity);
            _dbContext.SaveChanges();

            return MapProcessInstance(instanceEntity);
        }

        public ProcessInstance GetInstance(int instanceId)
        {
            WF_Process_m instance = _dbContext.WF_Process_m.SingleOrDefault(record => record.ID == instanceId);
            if (instance == null)
            {
                throw new InvalidOperationException($"Process instance '{instanceId}' was not found.");
            }

            return MapProcessInstance(instance);
        }

        public ProcessExecutionStep AddStep(ProcessExecutionStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            var entity = new WF_ProcessExecutionStep_m
            {
                ID = step.ID,
                ProcessID = step.ProcessInstanceID,
                ProcessNodeID = step.ProcessNodeID,
                PathID = step.PathID,
                Done = step.Done,
                RegisterDateTime = step.RegisterDateTime,
                DoneDateTime = step.DoneDateTime,
                PreviousExecutionStepID = string.Join(",", step.PreviousStepIds ?? Array.Empty<Guid>()),
                Data = step.Data,
                RegUserID = step.RegUserID,
                RegDate = ServerTime.PersianNow().ToStandardDateTime(),
                ModifyUserID = step.ModifyUserID,
                ModifyDate = ServerTime.PersianNow().ToStandardDateTime(),
                RegCompanyID = step.RegCompanyID
            };

            _dbContext.WF_ProcessExecutionStep_m.Add(entity);
            _dbContext.SaveChanges();

            return MapExecutionStep(entity);
        }

        public ProcessExecutionStep GetStep(Guid stepId)
        {
            WF_ProcessExecutionStep_m step = _dbContext.WF_ProcessExecutionStep_m
                .Include(x => x.MD_ProcessNode_m).SingleOrDefault(record => record.ID == stepId);
            if (step == null)
            {
                throw new InvalidOperationException($"Execution step '{stepId}' was not found.");
            }

            return MapExecutionStep(step);
        }

        public IEnumerable<ProcessExecutionStep> GetStepsForInstance(int instanceId)
        {
            return _dbContext.WF_ProcessExecutionStep_m
                .Where(step => step.ProcessID == instanceId)
                .ToList()
                .Select(MapExecutionStep);
        }

        public void UpdateStep(ProcessExecutionStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException(nameof(step));
            }

            WF_ProcessExecutionStep_m entity = _dbContext.WF_ProcessExecutionStep_m.SingleOrDefault(record => record.ID == step.ID);
            if (entity == null)
            {
                throw new InvalidOperationException($"Execution step '{step.ID}' was not found for update.");
            }

            entity.Done = step.Done;
            entity.DoneDateTime = step.DoneDateTime ?? step.CompletedOnUtc?.Ticks;
            entity.PreviousExecutionStepID = string.Join(",", step.PreviousStepIds ?? Array.Empty<Guid>());
            entity.Data = step.Data;
            entity.ModifyUserID = step.ModifyUserID;
            entity.ModifyDate = ServerTime.PersianNow().ToStandardDateTime();
            entity.RegisterDateTime = step.RegisterDateTime;
            entity.PathID = step.PathID;
            entity.ProcessNodeID = step.ProcessNodeID;
            entity.ProcessID = step.ProcessInstanceID;

            _dbContext.SaveChanges();
        }

        public IList<Process> LoadProcessesFromDatabase()
        {
            var processes = _dbContext.MD_Process_m
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_ForkNextProcessNode_m))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_ProcessNodeKind_h))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_UserTaskNode_m.MD_UserTaskRegistrationType_h))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_TimerNode_m))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_ServiceTaskNode_m))
                .Include(process => process.MD_VoucherKind_h)
                .ToList();

            var result = new List<Process>();
            foreach (MD_Process_m dbProcess in processes)
            {
                result.Add(MapProcess(dbProcess));
            }

            return result;
        }

        public IList<Process> ProcessByID(int id)
        {
            var processes = _dbContext.MD_Process_m
                .Where(x => x.ID == id)
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_ForkNextProcessNode_m))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_ProcessNodeKind_h))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_UserTaskNode_m.MD_UserTaskRegistrationType_h))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_TimerNode_m))
                .Include(process => process.MD_ProcessNode_m.Select(node => node.MD_ServiceTaskNode_m))
                .Include(process => process.MD_VoucherKind_h)
                .ToList();

            var result = new List<Process>();
            foreach (MD_Process_m dbProcess in processes)
            {
                result.Add(MapProcess(dbProcess));
            }

            return result;
        }




        public Process ActiveProcess(int voucherKindId, IInternalExecutionContext context)
        {
            var dbProcess = _dbContext.MD_Process_m
                .AsNoTracking()
                .Where(p => p.Active && p.VoucherKindID == voucherKindId)
                .Include(p => p.MD_VoucherKind_h)
                .FirstOrDefault();

            if (dbProcess == null)
                return null;


            var nodes = _dbContext.MD_ProcessNode_m
                .AsNoTracking()
                .Where(n => n.ProcessID == dbProcess.ID)
                .Include(n => n.MD_ForkNode_m)
                .Include(n => n.MD_ForkNextProcessNode_m)
                .Include(n => n.MD_ProcessNodeKind_h)
                .Include(n => n.MD_TimerNode_m)
                .Include(n => n.MD_ServiceTaskNode_m)
                .Include(n => n.MD_UserTaskNode_m.MD_UserTaskRegistrationType_h)
                .ToList();

            dbProcess.MD_ProcessNode_m = nodes;

            return MapProcess(dbProcess);
        }

        public ProcessNode ProcessNode(int nodeID)
        {
            var node = _dbContext.MD_ProcessNode_m
                .FirstOrDefault(x => x.ID == nodeID);

            return MapProcessNode(node);
        }

        private Process MapProcess(MD_Process_m dbProcess)
        {
            var process = new Process
            {
                ID = dbProcess.ID,
                Name = dbProcess.Name,
                VoucherKindID = dbProcess.VoucherKindID,
                Version = dbProcess.Version,
                Active = dbProcess.Active,
                RegUserID = dbProcess.RegUserID,
                RegDate = ParseDate(dbProcess.RegDate) ?? DateTime.UtcNow,
                ModifyUserID = dbProcess.ModifyUserID,
                ModifyDate = ParseDate(dbProcess.ModifyDate),
                RegCompanyID = dbProcess.RegCompanyID,
            };

            var nodeLookup = new Dictionary<int, ProcessNode>();
            foreach (MD_ProcessNode_m dbNode in dbProcess.MD_ProcessNode_m)
            {
                ProcessNode node = MapProcessNode(dbNode);
                node.Process = process;
                nodeLookup[node.ID] = node;
                process.Nodes.Add(node);
            }

            foreach (ProcessNode node in nodeLookup.Values)
            {
                if (node.NextProcessNodeID.HasValue && nodeLookup.TryGetValue(node.NextProcessNodeID.Value, out ProcessNode next))
                {
                    node.NextProcessNode = next;
                }
            }

            foreach (MD_ProcessNode_m dbNode in dbProcess.MD_ProcessNode_m)
            {
                ProcessNode node = nodeLookup[dbNode.ID];

                if (dbNode.MD_ForkNode_m != null)
                {
                    foreach (MD_ForkNextProcessNode_m fork in dbNode.MD_ForkNode_m.MD_ForkNextProcessNode_m)
                    {
                        var transition = new ForkNextProcessNode
                        {
                            ID = fork.ID,
                            ForkNodeID = fork.ForkNodeID,
                            NextProcessNodeID = fork.NextProcessNodeID,
                            Title = fork.Title,
                            Condition = fork.Condition,
                            DesignerLinkPath = fork.DesignerLinkPath,
                            NextProcessNode = nodeLookup.TryGetValue(fork.NextProcessNodeID, out ProcessNode target) ? target : null
                        };

                        transition.ForkNode = new ForkNode()
                        {
                            ProcessNode = node
                        };
                        node.ForkNextProcessNodes.Add(transition);
                    }
                }
            }

            return process;
        }

        private ProcessNode MapProcessNode(MD_ProcessNode_m dbNode)
        {
            var node = new ProcessNode
            {
                ID = dbNode.ID,
                Name = dbNode.Name,
                ProcessID = dbNode.ProcessID,
                NodeKindID = dbNode.NodeKindID,
                NextProcessNodeID = dbNode.NextProcessNodeID,
                DesignerLocation = dbNode.DesignerLocation,
                DesignerLinkPath = dbNode.DesignerLinkPath,
                NodeKind = (ProcessNodeKind)dbNode.NodeKindID
            };

            if (dbNode.MD_TimerNode_m != null)
            {
                node.Settings["delay"] = dbNode.MD_TimerNode_m.DelayDateTimeExpression ?? string.Empty;
            }

            if (dbNode.MD_UserTaskNode_m != null)
            {
                node.Settings["registrationType"] = dbNode.MD_UserTaskNode_m.MD_UserTaskRegistrationType_h?.Name ?? string.Empty;

                if (dbNode.MD_UserTaskNode_m.IsStartTask)
                {
                    node.Settings["IsStartTask"] = dbNode.MD_UserTaskNode_m?.IsStartTask.ToString() ?? string.Empty;
                }
            }

            if (dbNode.MD_ServiceTaskNode_m != null)
            {
                node.Settings["metadata"] = dbNode.MD_ServiceTaskNode_m.Metadata ?? string.Empty;
                node.Settings["serviceType"] = dbNode.MD_ServiceTaskNode_m.TypeId.ToString(CultureInfo.InvariantCulture);
            }

            return node;
        }


        private VoucherKind MapVoucherKind(MD_VoucherKind_h dbVoucher)
        {
            if (dbVoucher == null)
            {
                return null;
            }

            return new VoucherKind
            {
                ID = dbVoucher.ID,
                Name = dbVoucher.Name,
                Remark = dbVoucher.Remark,
                Type = dbVoucher.Type,
                TypeDesc = dbVoucher.TypeDesc,
                AttachFormUrlID = dbVoucher.AttachFormUrlID,
                IconEnum = dbVoucher.IconEnum,
                WorkSheetPermissionItemID = dbVoucher.WorkSheetPermissionItemID,
                ServiceAssemblyName = dbVoucher.ServiceAssemblyName,
                GetMethodName = dbVoucher.GetMethodName,
                LatinName = dbVoucher.LatinName,
                ClientCenter = dbVoucher.ClientCenter,
                Company = dbVoucher.Company,
                IsGoverment = dbVoucher.IsGoverment
            };
        }

        private ProcessInstance MapProcessInstance(WF_Process_m entity)
        {
            return new ProcessInstance
            {
                ID = entity.ID,
                ProcessID = entity.ProcessMetaDataID,
                VoucherID = entity.VoucherRowID,
                IsClosed = entity.Closed,
            };
        }

        private ProcessExecutionStep MapExecutionStep(WF_ProcessExecutionStep_m entity)
        {
            var step = new ProcessExecutionStep
            {
                ID = entity.ID,
                ProcessID = entity.ProcessID,
                ProcessNodeID = entity.ProcessNodeID,
                ProcessInstanceID = entity.ProcessID,
                PathID = entity.PathID,
                Done = entity.Done,
                RegisterDateTime = entity.RegisterDateTime,
                DoneDateTime = entity.DoneDateTime,
                Data = entity.Data,
                RegUserID = entity.RegUserID,
                RegDate = ParseDate(entity.RegDate) ?? DateTime.UtcNow,
                ModifyUserID = entity.ModifyUserID,
                ModifyDate = ParseDate(entity.ModifyDate),
                RegCompanyID = entity.RegCompanyID,
                CreatedOnUtc = entity.RegisterDateTime > 0 ? new DateTime(entity.RegisterDateTime, DateTimeKind.Utc) : DateTime.MinValue,
                CompletedOnUtc = entity.DoneDateTime.HasValue ? new DateTime(entity.DoneDateTime.Value, DateTimeKind.Utc) : (DateTime?)null,
            };

            if (!string.IsNullOrWhiteSpace(entity.PreviousExecutionStepID))
            {
                foreach (string id in entity.PreviousExecutionStepID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (Guid.TryParse(id, out Guid parsed))
                    {
                        step.PreviousStepIds.Add(parsed);
                    }
                }
            }

            return step;
        }
        private static DateTime? ParseDate(string dateValue)
        {
            if (string.IsNullOrWhiteSpace(dateValue))
            {
                return null;
            }

            if (DateTime.TryParse(dateValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
