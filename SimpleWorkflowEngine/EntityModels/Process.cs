using System;

namespace SimpleWorkflowEngine.EntityModels
{
    public class Process
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int VoucherKindID { get; set; }
        public int Version { get; set; }
        public bool Active { get; set; } = true;
        public int RegUserID { get; set; }
        public DateTime RegDate { get; set; }
        public int? ModifyUserID { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int RegCompanyID { get; set; }

        public VoucherKind VoucherKind { get; set; }
    }
}