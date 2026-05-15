using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface IBillInfoRepository : IBaseRepository<BillInfo>
    {
        Task<List<BillInfo>> GetById(Guid Id);
        Task<BillInfo> AddAsync(BillInfo user);
        Task<bool> Remove (BillInfo data);
        Task<bool> RemoveByBillId(Guid Id);
    }
}
