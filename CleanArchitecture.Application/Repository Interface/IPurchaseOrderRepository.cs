using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface IPurchaseOrderRepository
    {
        Task<bool> Add(PurchaseOrder data);
        Task<List<PurchaseOrderDetailModel>> getPurchaseOrder ( Guid purchaseId ); 
    }
}
