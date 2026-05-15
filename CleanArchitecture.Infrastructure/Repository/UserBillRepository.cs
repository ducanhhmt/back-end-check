using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MassTransit.Futures.Contracts;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class UserBillRepository : IUserBillRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<CachedUserBillDetail> _userBillCache;
        private readonly IMongoCollection<CachedUserBillPerStateListing> _userBillListCache;
        // Stampede protection — 1 request query SQL, còn lại chờ rồi đọc cache
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();
        public UserBillRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _userBillCache = database.GetCollection<CachedUserBillDetail>("cache_userBill_detail");
            _userBillListCache = database.GetCollection<CachedUserBillPerStateListing>("cache_userBill_perState_list");
            EnsureIndexes();
        }
        /// <summary>
        /// HÀM SỬ DỤNG ĐỂ ĐÁNH INDEX CHO MONGODB
        /// </summary>
        private void EnsureIndexes()
        {
            void Ttl<T>(IMongoCollection<T> col,
                System.Linq.Expressions.Expression<Func<T, object>> f) =>
                col.Indexes.CreateOne(new CreateIndexModel<T>(
                    Builders<T>.IndexKeys.Ascending(f),
                    new CreateIndexOptions { ExpireAfter = TimeSpan.Zero }));

            void Unique<T>(IMongoCollection<T> col,
                System.Linq.Expressions.Expression<Func<T, object>> f) =>
                col.Indexes.CreateOne(new CreateIndexModel<T>(
                    Builders<T>.IndexKeys.Ascending(f),
                    new CreateIndexOptions { Unique = true }));

            Ttl(_userBillCache, x => x.ExpireAt);
            Ttl(_userBillListCache, x => x.ExpireAt);
            Unique(_userBillCache, x => x.CacheKey);
            Unique(_userBillListCache, x => x.CacheKey);
        }

        public async Task<List<UserBill>> GetAll()
        {           
            return new List<UserBill>();
        }

        public async Task<UserBill> GetById(Guid BillId)
        {
            var query = await _context.userBill.FirstOrDefaultAsync(n => n.Id == BillId);
            return query;
        }

        public async Task<List<UserBill>> GetByAccount(Guid userId)
        {
            var query = await _context.userBill.Where(x => x.UserId == userId).ToListAsync();
            return query;
        }

        public async Task<UserBill> Add(UserBill data)
        {
            _context.userBill.Add(data);
            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<UserBill> Update(UserBill data)
        {
            _context.userBill.Update(data);
            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<bool> Remove(UserBill data)
        {
            _context.userBill.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }

        // ================================================================
        // PRIVATE HELPERS
        // ================================================================
        private DateTime Expire() => DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));
        public async Task<UserBillDTO> GetCachedUserBillInfo(Guid userBillId)
        {
            var key = $"userBilldetail:{userBillId}";
            var cached = await _userBillCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null && cached.Data != null) 
                return cached.Data;
            return new UserBillDTO();
        }

        public async Task<List<UserBillModel>> GetCachedUserBillListingPerState(Guid userId, int State, int pageIndex)
        {
            var key = $"userBill:{userId}:state:{State}:page:{pageIndex}:";
            var cached = await _userBillListCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null && cached.Items.Count > 0)
                return cached.Items;
            return new List<UserBillModel>();
        }

        public async Task WriteUserBillDetailCache(string key, UserBillDTO data)
            => await _userBillCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedUserBillDetail { Id = ObjectId.GenerateNewId(), CacheKey = key, Data = data, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        public async Task WriteUserBillPerStateListCache(string key, List<UserBillModel> items)
            => await _userBillListCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedUserBillPerStateListing { Id = ObjectId.GenerateNewId(), CacheKey = key, Items = items, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });


        public async Task InvalidateUserBilllCache(Guid id)
            => await _userBillCache.DeleteOneAsync(x => x.CacheKey == $"userBilldetail:{id}");

        public async Task InvalidateUserBillListCache(Guid userId, int State)
            => await _userBillListCache.DeleteManyAsync(
            Builders<CachedUserBillPerStateListing>.Filter.Regex(
                x => x.CacheKey, new BsonRegularExpression($"^userBill:{userId}:state:{State}:")));


        public class CachedUserBillDetail
        {
            public ObjectId Id { get; set; }
            public string CacheKey { get; set; } = default!;
            public UserBillDTO Data { get; set; } = default!;
            public DateTime ExpireAt { get; set; }
        }

        public class CachedUserBillPerStateListing
        {
            public ObjectId Id { get; set; }
            public string CacheKey { get; set; } = default!;
            public List<UserBillModel> Items { get; set; } = [];
            public DateTime ExpireAt { get; set; }
        }
    }
}
