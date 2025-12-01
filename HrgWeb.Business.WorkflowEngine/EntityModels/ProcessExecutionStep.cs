using System;
using System.Collections.Generic;

namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class ProcessExecutionStep
    {
        public Guid ID { get; set; }

        public int ProcessID { get; set; }

        public int ProcessNodeID { get; set; }

        public int ProcessInstanceId { get; set; }

        public int NodeId
        {
            get => ProcessNodeID;
            set => ProcessNodeID = value;
        }

        public Guid PathID { get; set; }

        public Guid Id
        {
            get => ID;
            set => ID = value;
        }

        public Guid PathId
        {
            get => PathID;
            set => PathID = value;
        }

        public bool Done { get; set; }

        public bool IsCompleted
        {
            get => Done;
            set => Done = value;
        }

        public long RegisterDateTime { get; set; }

        public long? DoneDateTime { get; set; }

        public string PreviousExecutionStepID { get; set; }

        public string Data { get; set; }

        public int RegUserID { get; set; }

        public DateTime RegDate { get; set; }

        public int? ModifyUserID { get; set; }

        public DateTime? ModifyDate { get; set; }

        public int RegCompanyID { get; set; }

        public ProcessNode ProcessNode { get; set; }
        public Process Process { get; set; }

        public IList<Guid> PreviousStepIds { get; } = new List<Guid>();

        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? CompletedOnUtc { get; set; }
    }
}
