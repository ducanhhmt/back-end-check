using CleanArchitecture.Domain.Entities;
using System.Text.Json.Serialization;

namespace CleanArchitecture.Application.Model
{
    public class UserBillModel
    {
        public Guid Id { get; set; }      
        public int TotalPrice { get; set; }
        public int State { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int itemsCount { get; set; } = 0;
        public ProductPreviewItems Items { get; set; }
        [JsonIgnore]
        public int TotalPages { get; set; }
    }

    public class ProductPreviewItems
    {
        public string Name { get; set; }
        public string Thumbnail { get; set; }
    }
}
