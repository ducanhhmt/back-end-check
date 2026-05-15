using CleanArchitecture.Application.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure
{
    public class CacheData : IMemCacheData
    {
        #region sử dụng MemCache
        private readonly IMemoryCache _memoryCache;

        public CacheData(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set<T>(string key, T obj, TimeSpan time)
        {
            _memoryCache.Set(key, obj, time);
        }

        public T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
        #endregion

        #region Sử dụng Redis + DistributedCache
        //private readonly IDistributedCache _cache;
        //public CacheData(IDistributedCache cache)
        //{
        //    _cache = cache;
        //}

        //public async Task<T?> Get<T>(string key)
        //{
        //    var data = await _cache.GetStringAsync(key);
        //    return data == null ? default : JsonSerializer.Deserialize<T>(data);
        //}

        //public async Task Set<T>(string key, T value, TimeSpan ttl)
        //{
        //    var options = new DistributedCacheEntryOptions
        //    {
        //        AbsoluteExpirationRelativeToNow = ttl
        //    };

        //    var json = JsonSerializer.Serialize(value);
        //    await _cache.SetStringAsync(key, json, options);
        //}

        //public Task Remove(string key)
        //{
        //    return _cache.RemoveAsync(key);
        //}
        #endregion
    }
}
