using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Series { get; private set; }
        public int NxbId { get; private set; }
        public int CategoriesId { get; private set; }
        public int Weight { get; private set; }
        public int Price {  get; private set; }
        public int ImportPrice { get; private set; }
        public int PublisherPrice { get; private set; }
        public int TotalStock { get; private set; }
        public int Quantity { get; private set; }
        public int Discount { get; private set; }
        public DateTime? DateCreated { get; private set; }
        public string? Description { get; private set; }
        public string? ThumbnailIMG { get; private set; }
        protected Product() { }
        public Product(string name, string series, int nxbid, int categoryId, int weight,
            int importprice, int price, int publisherprice, int totalStock,  int quantity, int discount, string? description, string? thumbnailimg)
        {
            CreateInfo(name, series, nxbid, categoryId, weight, importprice, price, publisherprice, totalStock, quantity, discount, description, thumbnailimg);
        }

        public void CreateInfo(string name, string series, int nxbid, int categoryId, int weight,
            int importprice, int price, int publisherprice, int totalStock, int quantity, int discount, string description, string thumbnail)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Không có thông tin tên sản phẩm");
            if (price == 0)
                throw new Exception("Không có thông tin giá sản phẩm");

            Name = name;
            Series = series;
            NxbId = nxbid;
            CategoriesId = categoryId;
            Weight = weight;
            ImportPrice = importprice;
            Price = price;
            PublisherPrice = publisherprice;
            TotalStock = Math.Max(0, totalStock);
            Quantity = Math.Max(0, quantity);
            Discount = discount;
            DateCreated = DateTime.Now;
            Description = description;
            ThumbnailIMG = thumbnail;
        }

        public void UpdateInfo(string name, string series, int nxbid, int categoryId, int weight,
            int importprice, int price, int publisherprice, int quantity, int discount, string description, string thumbnail)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Không có thông tin tên sản phẩm");
            if (price == 0)
                throw new Exception("Không có thông tin giá sản phẩm");

            Name = name;
            Series = series;
            NxbId = nxbid;
            CategoriesId = categoryId;
            Weight = weight;
            ImportPrice = importprice;
            Price = price;
            PublisherPrice = publisherprice;
            Quantity = quantity;
            Discount = discount;
            Description = description;
            ThumbnailIMG = thumbnail;
        }

        /// Tính toán số lượng sản phẩm còn có thể bán //
        public void UpdateStock(int quantity)
        {
            if (quantity < 0)
                throw new Exception("Số lượng không hợp lệ");

            Quantity = quantity;
        }
        public void CalculateNewImportPrice(int newQuantity, int newImportPrice)
        {
            if (newQuantity <= 0)
                throw new Exception("Số lượng nhập không hợp lệ");

            if (newImportPrice <= 0)
                throw new Exception("Giá nhập không hợp lệ");

            int totalQuantity = TotalStock + newQuantity;

            int totalValue =
                (TotalStock * ImportPrice) +
                (newQuantity * newImportPrice);

            ImportPrice = totalValue / totalQuantity;
            TotalStock = totalQuantity;
            Quantity += newQuantity;
        }

        public void RollbackImportPrice(int cancelQuantity, int cancelImportPrice)
        {
            // Hoàn lại số lượng
            Quantity = Math.Max(0, Quantity - cancelQuantity);
            TotalStock = Math.Max(0, TotalStock - cancelQuantity);

            // Tính lại giá nhập trung bình sau khi bỏ lô hàng bị hủy
            var remainQuantity = TotalStock; // sau khi đã trừ
            if (remainQuantity <= 0)
            {
                ImportPrice = 0;
                return;
            }

            // Công thức ngược lại của CalculateNewImportPrice
            // TotalCost hiện tại = ImportPrice * (remainQuantity + cancelQuantity)
            // TotalCost cần hoàn = cancelQuantity * cancelImportPrice
            // ImportPrice mới = (TotalCost hiện tại - TotalCost hoàn) / remainQuantity
            var totalCostBefore = ImportPrice * (remainQuantity + cancelQuantity);
            var totalCostRollback = cancelQuantity * cancelImportPrice;
            ImportPrice = Math.Max(0, (totalCostBefore - totalCostRollback) / remainQuantity);
        }
    }
}
