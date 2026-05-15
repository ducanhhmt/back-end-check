namespace CleanArchitecture.Application.Model
{
    public class SupplierInfoModel
    {
        public int Id { get;  set; }
        public string Name { get;  set; }
        public string? Phone { get;  set; }
        public string? Email { get;  set; }
        public string? Address { get; set; }
        public int Debt { get; set; }
        public int totalOrders { get; set; }
        public int totalAmount { get; set; }
    }
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
