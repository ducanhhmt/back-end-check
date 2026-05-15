using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; private set; }
        public Guid PurchaseId { get; private set; }
        public Guid ProductId { get; private set; }
        public int ImportPrice { get; private set; }
        public int Quantity { get; private set; }
        public int TotalPrice { get; private set; }

        protected PurchaseOrder() { }
        public PurchaseOrder(Guid PurchaseId, Guid ProductId, int ImportPrice, int Quantity, int TotalPrice)
        {
            CreateInfo(PurchaseId, ProductId, ImportPrice, Quantity, TotalPrice);
        }

        public void CreateInfo(Guid purchaseId, Guid productId, int importPrice, int quantity, int totalPrice)
        {
            if (string.IsNullOrWhiteSpace(purchaseId.ToString()))
                throw new Exception("Không có Thông tin đơn nhập hàng");
            if (string.IsNullOrWhiteSpace(productId.ToString()))
                throw new Exception("Không có thông tin sản phẩm");
            PurchaseId = purchaseId;
            ProductId = productId;
            ImportPrice = importPrice;
            Quantity = quantity;
            TotalPrice = totalPrice;
        }
    }
}
