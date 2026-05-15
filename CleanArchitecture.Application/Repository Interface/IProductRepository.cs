using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        //Task<Product> GetById(Guid Id);
        Task<List<ProductUserListingDto>> GetUserListing(string? keyword, int? categoryId, int page, int pageSize, string stockFilter, int? nxbId = null, int? minPrice = null, int? maxPrice = null);
        Task<ProductUserDetailDto?> GetUserDetail(Guid id);
        Task<List<ProductAdminListingDto>> GetAdminListing(int pageIndex, int pageSize, int? categoryId = null, int? nxbId = null, string stockFilter = "all", string? Keyword = null);
        Task<ProductAdminDetailDto?> GetAdminDetail(Guid id);
        Task<Product> GetProductDetail(Guid id);
        Task<List<ProductAdminFiltered>> GetAdminFiltered(int pageIndex, int pageSize, string? Keyword = null);
        Task<Product> AddAsync(Product data);
        Task<Product> UpdateAsync(Product data);
        Task<bool> IsValid(string name);
        //Task<bool> Remove(Product data);
        Task<bool> Delete(Guid id);
    }
}
