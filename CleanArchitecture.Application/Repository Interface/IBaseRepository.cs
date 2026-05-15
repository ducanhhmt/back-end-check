using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Repository_Interface
{
    public interface IBaseRepository<T>
    {
        Task<List<T>> GetAll();
    }
}
