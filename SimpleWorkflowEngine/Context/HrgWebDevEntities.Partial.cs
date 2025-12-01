using System.Data.Entity;

namespace HrgWeb.Business.WorkflowEngine.Context
{
    public partial class HrgWebDevEntities
    {
        public HrgWebDevEntities(string connectionString)
            : base(connectionString)
        {
        }
    }
}
