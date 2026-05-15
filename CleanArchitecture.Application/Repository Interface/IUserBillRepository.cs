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
    public interface IUserBillRepository : IBaseRepository<UserBill>
    {
        Task<UserBill> GetById(Guid BillId);
        Task<List<UserBill>> GetByAccount(Guid userId);
        Task<UserBill> Add(UserBill user);
        Task<UserBill> Update(UserBill user);
        Task<bool> Remove (UserBill data);
        // Cache
        Task<UserBillDTO> GetCachedUserBillInfo(Guid userBillId);
        Task<List<UserBillModel>> GetCachedUserBillListingPerState(Guid userId, int State, int pageIndex);
        Task WriteUserBillDetailCache(string key, UserBillDTO data);
        Task WriteUserBillPerStateListCache(string key, List<UserBillModel> items);
        Task InvalidateUserBilllCache(Guid id);
        Task InvalidateUserBillListCache(Guid userId, int State);
    }
}
