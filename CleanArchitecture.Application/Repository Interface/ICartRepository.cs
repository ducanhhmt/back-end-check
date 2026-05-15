using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<List<UserCartModel>> GetCartByUserId(Guid userId);
        Task<Cart> checkProductExitsOnCart(Guid userId, Guid productId);
        Task<Cart> GetById(int id);
        Task<Cart> Add(Cart data);
        Task<Cart> Update(Cart data);
        Task<bool> Remove(RemoveCartbyUserId data);
        Task<bool> RemoveCartOnUser(Guid userId, Guid ProductId);
    }
}
