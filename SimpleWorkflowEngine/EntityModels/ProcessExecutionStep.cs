using System;

namespace SimpleWorkflowEngine.EntityModels
{
    public class ProcessExecutionStep
    {
        public Guid ID { get; set; }
        
        public int ProcessID { get; set; }
        
        public int ProcessNodeID { get; set; }
        
        public Guid PathID { get; set; }
        
        public bool Done { get; set; }
        
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
    }
}