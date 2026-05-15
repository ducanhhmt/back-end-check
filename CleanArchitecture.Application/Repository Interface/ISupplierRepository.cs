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
    public interface ISupplierRepository 
    {
        Task<List<SupplierViewModel>> GetAll();
        Task<SupplierInfoModel> GetInfoModel( int id);
        Task<bool> ClearCached(int supplierid);
    }
}
