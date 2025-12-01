namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    /// <summary>
    /// Simple immutable voucher implementation.
    /// </summary>
    public sealed class WorkflowVoucher : IWorkflowVoucher
    {
        public WorkflowVoucher(int id, string number, string date, int kind = 0)
        {
            ID = id;
            Number = number;
            Date = date;
            Kind = kind;
        }

        public int ID { get; set; }

        public string Number { get; set; }

        public string Date { get; set; }

        public int Kind { get; set; }
    }
}
