using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<UserDto> GetById(string Id);
        Task<User> GetByAccount(string account);
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdateUserProfile(Guid id, string firstName, string lastName, string email, string phone, bool gender, DateTime birthday);
        Task<bool> UpdateUserAddress(Guid id, string Address);
        Task<bool> ChangePassword(Guid id, string passwordHash, string passwordSalt);
        Task<(string hash, string salt)?> GetPasswordByUserId(Guid userId);
        Task<bool> IsValid(string account);
        Task<bool> Remove (User data);
        Task<bool> Delete(string id);
        //Task<bool> CachedUserLogin()
    }
}
