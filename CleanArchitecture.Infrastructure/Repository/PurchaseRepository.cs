using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<CachedPurchaseDetail> _purchaseDetailCache;
        private readonly IMongoCollection<CachedPurchaseListing> _purchaseListCache;
        // Stampede protection — 1 request query SQL, còn lại chờ rồi đọc cache
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();
        public PurchaseRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _purchaseDetailCache = database.GetCollection<CachedPurchaseDetail>("cache_purchase_detail");
            _purchaseListCache = database.GetCollection<CachedPurchaseListing>("cache_purchase_list");
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

            Ttl(_purchaseDetailCache, x => x.ExpireAt);
            Ttl(_purchaseListCache, x => x.ExpireAt);
            Unique(_purchaseDetailCache, x => x.CacheKey);
            Unique(_purchaseListCache, x => x.CacheKey);
        }

        public async Task<List<PurchaseViewModel>> PaginationFilter(string keyword, int pageIndex, int pageSize)
        {
            var key = $"list_purchase:key:{keyword}:p:{pageIndex}:s:{pageSize}";
            var cached = await _purchaseListCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null && cached.Items.Count > 0) return cached.Items;
            var items = await _context.Database
                .SqlQueryRaw<PurchaseViewModel>(
                    "EXEC sp_GetPurchaseListing @Keyword, @Skip, @Take",
                    new SqlParameter("@Keyword", (object?)keyword ?? DBNull.Value),
                    new SqlParameter("@Skip", (pageIndex - 1) * pageSize),
                    new SqlParameter("@Take", pageSize))
                .ToListAsync();
            await WritePurchaseListCache(key, items);
            return items;
        }

        public async Task<Purchase> GetbyId(Guid id)
        {
            return await _context.purchase.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PurchaseDetailModel?> GetInfoModel(Guid id)
        {
            var key = $"purchase:detail:{id}";
            // Chỉ kiểm tra cache, không ghi
            var cached = await _purchaseDetailCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null) return cached.Data;

            var sem = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();
            try
            {
                // Double-check
                cached = await _purchaseDetailCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
                if (cached != null) return cached.Data;
                var data = (await _context.Database.SqlQueryRaw<PurchaseDetailModel>("EXEC sp_GetPurchaseById @Id",
                        new SqlParameter("@Id", id)).ToListAsync()).FirstOrDefault();
                return data; // Không ghi cache
            }
            finally
            {
                sem.Release();
                _locks.TryRemove(id, out _);
            }
        }

        public async Task CachedPurchaseDetailData(string key, PurchaseDetailModel data)
        {
            WritePurchaseDetailCache(key, data);
        }

        public async Task<bool> IsValid(string name)
        {
            return await _context.products.AnyAsync(u => u.Name == name);
        }

        //public async Task<Product> UpdateAsync(Product dto)
        //{
        //    await _context.SaveChangesAsync();
        //    var tasks = new List<Task>
        //    {
        //        InvalidateUserDetailCache(dto.Id),
        //        InvalidateUserListCache(dto.CategoriesId),
        //        InvalidateAdminListCache(dto.CategoriesId),
        //    };

        //    if (dto.CategoriesId != dto.CategoriesId)
        //    {
        //        tasks.Add(InvalidateUserListCache(dto.CategoriesId));
        //        tasks.Add(InvalidateAdminListCache(dto.CategoriesId));
        //    }
        //    await Task.WhenAll(tasks);
        //    return dto;
        //}

        //public async Task<bool> Delete(Guid id)
        //{
        //    var product = await _context.products.FindAsync(id);
        //    if (product == null) return false;

        //    _context.products.Remove(product);
        //    await _context.SaveChangesAsync();

        //    await Task.WhenAll(
        //        InvalidateUserDetailCache(id),
        //        InvalidateUserListCache(product.CategoriesId),
        //        InvalidateAdminListCache(product.CategoriesId));

        //    return true;
        //}

        public async Task<Purchase> Add(Purchase data)
        {
            _context.purchase.Add(data);
            await _context.SaveChangesAsync();
            await Task.WhenAll(
                InvalidateAllPurchaseListCache()
            );
            return data;
        }

        public async Task<Purchase> Update(Purchase data)
        {
            _context.purchase.Update(data);
            await _context.SaveChangesAsync();
            await Task.WhenAll(
                InvalidateAllPurchaseListCache()
            );
            return data;
        }
        // ================================================================
        // PRIVATE HELPERS
        // ================================================================
        private DateTime Expire() => DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));

        private async Task WritePurchaseDetailCache(string key, PurchaseDetailModel data)
            => await _purchaseDetailCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedPurchaseDetail { Id = ObjectId.GenerateNewId(), CacheKey = key, Data = data, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task WritePurchaseListCache(string key, List<PurchaseViewModel> items)
            => await _purchaseListCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedPurchaseListing { Id = ObjectId.GenerateNewId(), CacheKey = key, Items = items, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task InvalidatePurchaseDetailCache(Guid id)
            => await _purchaseDetailCache.DeleteOneAsync(x => x.CacheKey == $"purchase:detail:{id}");

        private async Task InvalidateAllPurchaseListCache()
            => await _purchaseListCache.DeleteManyAsync(
                Builders<CachedPurchaseListing>.Filter.Regex(
                    x => x.CacheKey, new BsonRegularExpression("^list_purchase:")));
    }

    public class CachedPurchaseDetail
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public PurchaseDetailModel Data { get; set; } = default!;
        public DateTime ExpireAt { get; set; }
    }

    public class CachedPurchaseListing
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public List<PurchaseViewModel> Items { get; set; } = [];
        public DateTime ExpireAt { get; set; }
    }
}
