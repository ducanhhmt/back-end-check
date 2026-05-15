using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class SupplierPurchase
    {
        public int Id { get; private set; }
        public int SupplierId { get; private set; }
        public Guid PurchaseId { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime DateModified { get; private set; }
        protected SupplierPurchase() { }
        public SupplierPurchase(int SupplierId, Guid PurchaseId, DateTime DateCreated, DateTime DateModified)
        {
            CreateInfo(SupplierId, PurchaseId, DateTime.Now, DateTime.Now);
        }

        public void CreateInfo(int supplierId, Guid purchaseId, DateTime dateCreated, DateTime dateModified)
        {
            if (string.IsNullOrWhiteSpace(SupplierId.ToString()))
                throw new Exception("Không có thông tin nhà cung cấp");
            if (string.IsNullOrWhiteSpace(PurchaseId.ToString()))
                throw new Exception("Không có thông tin đơn nhập hàng");
            SupplierId = supplierId;
            PurchaseId = purchaseId;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }
        public void updateState (DateTime dateModified)
        {
            DateModified = dateModified;
        }
    }
}
