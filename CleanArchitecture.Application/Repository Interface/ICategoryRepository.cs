using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        List<Category> GetAllCategories();
        //Task<User> GetById(string Id);
        //Task<User> GetByAccount(string account);
        //Task<User> AddAsync(User user);
        //Task<bool> UpdateAsync(User user);
        //Task<bool> IsValid(string account);
        //Task<bool> Remove (User data);
    }
}
