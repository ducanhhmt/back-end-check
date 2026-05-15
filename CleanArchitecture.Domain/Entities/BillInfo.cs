using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class BillInfo
    {
        public Guid Id { get; private set; }
        public Guid BillId { get; private set; }
        public Guid ProductId { get; private set; }
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public int Price { get; private set; }
        public int ImportPrice { get; private set; }
        public int TotalPrice { get; private set; }
        public string Thumbnail { get; private set; }
        protected BillInfo() { }
        public BillInfo(Guid BillId, Guid ProductId, string Name, int Quantity, int Price, int ImportPrice, int TotalPrice, string thumbnail)
        {
            UpdateInfo(BillId, ProductId, Name, Quantity, Price, ImportPrice, TotalPrice, thumbnail);           
        }

        public void UpdateInfo(Guid billId,Guid productId, string name, int quantity, int price, int importPrice, int totalPrice, string thumbnail)
        {
            if (string.IsNullOrWhiteSpace(billId.ToString()))
                throw new Exception("Không có Id hóa đơn");
            if (string.IsNullOrWhiteSpace(productId.ToString()))
                throw new Exception("Không có mã sản phẩm");
            BillId = billId;
            ProductId = productId;
            Name = name;
            Quantity = quantity;
            Price = price;
            ImportPrice = importPrice;
            TotalPrice = totalPrice;
            Thumbnail = thumbnail;
        }
    }  
}
