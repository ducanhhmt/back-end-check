using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Cache
{
    public interface IMemCacheData
    {
        void Set<T>(string key, T obj, TimeSpan time);
        T Get<T>(string key);
        void Remove(string key);

        //Task<T?> Get<T>(string key);
        //Task Set<T>(string key, T value, TimeSpan ttl);
        //Task Remove(string key);
    }

    public interface ISharedCacheData
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
        Task SetAsync<T>(string key, T data, TimeSpan? ttl = null, CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
    }
}
