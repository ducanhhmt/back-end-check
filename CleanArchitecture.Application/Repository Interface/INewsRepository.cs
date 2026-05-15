using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface INewsRepository : IBaseRepository<News>
    {
        Task<News> GetById(string Id);
        Task<List<News>> Search(string keyword);
        Task<bool> AddAsync(News student);
        Task<bool> UpdateAsync(News student);
        Task<bool> IsValid(string id);
        Task<bool> Delete(string id);
    }
}
