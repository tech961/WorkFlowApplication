namespace HrgWeb.Business.WorkflowEngine.Runtime
{
    public interface IWorkflowVoucher
    {
        int ID { get; set; }
        string Number { get; set; }
        string Date { get; set; }

        int Kind { get; set; }
    }
}
