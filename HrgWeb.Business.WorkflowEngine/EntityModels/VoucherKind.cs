namespace HrgWeb.Business.WorkflowEngine.EntityModels
{
    public class VoucherKind
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public int? Type { get; set; }
        public string TypeDesc { get; set; }
        public int? AttachFormUrlID { get; set; }
        public short? IconEnum { get; set; }
        public int WorkSheetPermissionItemID { get; set; }
        public string ServiceAssemblyName { get; set; }
        public string GetMethodName { get; set; }
        public string LatinName { get; set; }
        public bool ClientCenter { get; set; } = false;
        public bool Company { get; set; } = false;
        public bool IsGoverment { get; set; } = false;
    }
}