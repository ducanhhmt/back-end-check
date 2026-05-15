using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
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
    public class SupplierRepository : ISupplierRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<CachedSupplierDetail> _supplierDetailCache;
        private readonly IMongoCollection<CachedSupplierListing> _supplierListCache;
        // Stampede protection — 1 request query SQL, còn lại chờ rồi đọc cache
        private readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();
        public SupplierRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _supplierDetailCache = database.GetCollection<CachedSupplierDetail>("cache_supplier_detail");
            _supplierListCache = database.GetCollection<CachedSupplierListing>("cache_supplier_list");
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

            Ttl(_supplierDetailCache, x => x.ExpireAt);
            Ttl(_supplierListCache, x => x.ExpireAt);
            Unique(_supplierDetailCache, x => x.CacheKey);
            Unique(_supplierListCache, x => x.CacheKey);
        }

        public async Task<List<SupplierViewModel>> GetAll()
        {
            var key = $"list_supplier";
            var cached = await _supplierListCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null && cached.Items.Count > 0) return cached.Items;
            var items = await _context.Database
                .SqlQueryRaw<SupplierViewModel>("EXEC sp_GetLstSupplier").ToListAsync();
            await WriteSupplierListCache(key, items);
            return items;
        }

        public async Task<SupplierInfoModel?> GetInfoModel(int id)
        {
            var key = $"supplier:detail:{id}";
            // Fast path
            var cached = await _supplierDetailCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null) return cached.Data;

            // Stampede protection
            var sem = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();
            try
            {
                // Double-check sau khi có lock
                cached = await _supplierDetailCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
                if (cached != null) return cached.Data;
                var data = (await _context.Database.SqlQueryRaw<SupplierInfoModel>("EXEC sp_GetSupplierInfo @Id",
                        new SqlParameter("@Id", id)).ToListAsync()).FirstOrDefault();
                if (data == null) return null;
                await WriteSupplierDetailCache(key, data);
                return data;
            }
            finally
            {
                sem.Release();
                _locks.TryRemove(id, out _);
            }
        }


        public async Task<bool> IsValid(string name)
        {
            return await _context.products.AnyAsync(u => u.Name == name);
        }

        public async Task<bool> ClearCached ( int supplierid)
        {
            await Task.WhenAll(
                InvalidateSupplierDetailCache(supplierid),
                InvalidateSupplierListCache()
            );
            return true;
        }

        //public async Task<Product> AddAsync(Product dto)
        //{
        //    var product = new Product(
        //        dto.Name, dto.Series, dto.NxbId, dto.CategoriesId,
        //        dto.Weight, dto.ImportPrice, dto.PublisherPrice, dto.Price,
        //        dto.Quantity, dto.Discount, dto.Description, dto.ThumbnailIMG);
        //    _context.products.Add(product);
        //    await _context.SaveChangesAsync();
        //    await Task.WhenAll(
        //        InvalidateUserListCache(product.CategoriesId),
        //        InvalidateAdminListCache(product.CategoriesId));
        //    return product;
        //}

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


        // ================================================================
        // PRIVATE HELPERS
        // ================================================================
        private DateTime Expire() => DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));

        private async Task WriteSupplierDetailCache(string key, SupplierInfoModel data)
            => await _supplierDetailCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedSupplierDetail { Id = ObjectId.GenerateNewId(), CacheKey = key, Data = data, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task WriteSupplierListCache(string key, List<SupplierViewModel> items)
            => await _supplierListCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedSupplierListing { Id = ObjectId.GenerateNewId(), CacheKey = key, Items = items, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task InvalidateSupplierDetailCache(int id)
            => await _supplierDetailCache.DeleteOneAsync(x => x.CacheKey == $"supplier:detail:{id}");
 
        private async Task InvalidateSupplierListCache()
            => await _supplierListCache.DeleteManyAsync(
                Builders<CachedSupplierListing>.Filter.Regex(
                    x => x.CacheKey, new BsonRegularExpression("list_supplier")));
 
    }

    public class CachedSupplierDetail
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public SupplierInfoModel Data { get; set; } = default!;
        public DateTime ExpireAt { get; set; }
    }

    public class CachedSupplierListing
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public List<SupplierViewModel> Items { get; set; } = [];
        public DateTime ExpireAt { get; set; }
    }
}
