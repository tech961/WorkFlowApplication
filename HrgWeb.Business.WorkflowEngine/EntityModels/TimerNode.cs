namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class TimerNode
    {
        public int ID { get; set; }
        public string DelayDateTimeExpression { get; set; }

        public ProcessNode ProcessNode { get; set; }
    }
}