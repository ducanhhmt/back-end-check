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
    public interface IPurchaseRepository 
    {
        Task<List<PurchaseViewModel>> PaginationFilter( string keyword, int pageindex, int pagesize);
        Task<Purchase>GetbyId(Guid id);
        Task<PurchaseDetailModel> GetInfoModel( Guid id);
        Task<Purchase> Add(Purchase data);
        Task<Purchase> Update(Purchase data);
        Task CachedPurchaseDetailData(string key, PurchaseDetailModel data);
    }
}
