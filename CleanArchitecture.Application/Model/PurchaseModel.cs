using CleanArchitecture.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.Model
{
    public class PurchaseDetailModel
    {
        public Guid Id { get; set; }
        public string UserCreated { get; set; }
        public int SupplierId { get; set; }
        public int ImportPrice { get; set; }
        [NotMapped]
        public List<PurchaseOrderDetailModel> PurchaseItems { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int State { get; set; }
    }
    public class PurchaseOrderDetailModel
    {     
        public Guid ProductId { get; set; }
        public string Productname { get; set; }
        [JsonIgnore]
        public string? ThumbnailIMG { get; set; }
        public string Images => ThumbnailIMG?.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        public int ImportPrice { get; private set; }
        public int Quantity { get; private set; }
        public int TotalPrice { get; private set; }
    }

    public class PurchaseViewModel
    {
        public Guid Id { get; set; }
        public string UserCreated { get; set; }
        public string SupplierName { get; set; }
        public int ImportPrice { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int State { get; set; }
        [JsonIgnore]
        public int TotalRecords { get; set; }
        [JsonIgnore]
        public int TotalPages { get; set; }
    }

    public class PurchasePaginationRespone
    {
        public List<PurchaseViewModel> data { get; set; }
        public int TotalRecords { get; set; }
        public int pageCount { get; set; }
    }
}
