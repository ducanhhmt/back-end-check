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
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<CachedUserDetail> _userDetailCache;
        private readonly IMongoCollection<CachedUserListing> _userListCache;
        private readonly IMongoCollection<CachedAdminListing> _adminListCache;
        // Stampede protection — 1 request query SQL, còn lại chờ rồi đọc cache
        private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();
        public ProductRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _userDetailCache = database.GetCollection<CachedUserDetail>("cache_product_user_detail");
            _userListCache = database.GetCollection<CachedUserListing>("cache_product_user_list");
            _adminListCache = database.GetCollection<CachedAdminListing>("cache_product_admin_list");
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

            Ttl(_userDetailCache, x => x.ExpireAt);
            Ttl(_userListCache, x => x.ExpireAt);
            Ttl(_adminListCache, x => x.ExpireAt);
            Unique(_userDetailCache, x => x.CacheKey);
            Unique(_userListCache, x => x.CacheKey);
            Unique(_adminListCache, x => x.CacheKey);
        }

        public async Task<List<Product>> GetAll()
        {
            var lstData = await _context.products.Take(5000).ToListAsync();
            return lstData;
        }
        // ================================================================
        // USER — LISTING
        // ================================================================
        public async Task<List<ProductUserListingDto>> GetUserListing(
            string? keyword, int? categoryId, int page, int pageSize, string stockFilter,
            int? nxbId = null, int? minPrice = null, int? maxPrice = null)
        {
            var key = $"user:list:{categoryId}:nxb{nxbId ?? 0}:p{minPrice ?? 0}-{maxPrice ?? 0}:p{page}:s{pageSize}:key:{keyword}:stock:{stockFilter}";
            var cached = await _userListCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null && cached.Items.Count > 0) return cached.Items;
            var items = await _context.Database
                .SqlQueryRaw<ProductUserListingDto>("EXEC sp_GetProductUserListing @CategoriesId, @NxbId, @StockFilter, @MinPrice, @MaxPrice, @Keyword, @Skip, @Take",
                    new SqlParameter("@CategoriesId", (object?)categoryId ?? DBNull.Value),
                    new SqlParameter("@NxbId", (object?)nxbId ?? DBNull.Value),
                    new SqlParameter("@StockFilter", stockFilter),
                    new SqlParameter("@MinPrice", (object?)minPrice ?? DBNull.Value),
                    new SqlParameter("@MaxPrice", (object?)maxPrice ?? DBNull.Value),
                    new SqlParameter("@Keyword", (object?)keyword ?? DBNull.Value),
                    new SqlParameter("@Skip", (page - 1) * pageSize),
                    new SqlParameter("@Take", pageSize))
                .ToListAsync();
            await WriteUserListCache(key, items);
            return items;
        }

        // ================================================================
        // USER — DETAIL (Cache-Aside + Stampede Protection)
        // ================================================================
        public async Task<ProductUserDetailDto?> GetUserDetail(Guid id)
        {
            var key = $"user:detail:{id}";
            // Fast path
            var cached = await _userDetailCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null) return cached.Data;

            // Stampede protection
            var sem = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();
            try
            {
                // Double-check sau khi có lock
                cached = await _userDetailCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
                if (cached != null) return cached.Data;
                var data = (await _context.Database.SqlQueryRaw<ProductUserDetailDto>("EXEC sp_GetProductUserDetail @Id",
                        new SqlParameter("@Id", id)).ToListAsync()).FirstOrDefault();
                if (data == null) return null;
                await WriteUserDetailCache(key, data);
                return data;
            }
            finally
            {
                sem.Release();
                _locks.TryRemove(id, out _);
            }
        }

        // ================================================================
        // ADMIN — LISTING
        // ================================================================
        public async Task<List<ProductAdminListingDto>> GetAdminListing(int pageIndex, int pageSize,
             int? categoryId = null, int? nxbId = null, string stockFilter = "all", string? Keyword = null)
        {
            var key = $"admin:list:cat{categoryId ?? 0}:nxb{nxbId ?? 0}:p{pageIndex}:s{pageSize}:key:{Keyword}:stock:{stockFilter}";

            var cached = await _adminListCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null) return cached.Items;
            var items = await _context.Database
                .SqlQueryRaw<ProductAdminListingDto>(
                    "EXEC sp_GetProductAdminListing @CategoriesId, @NxbId, @StockFilter, @Keyword, @Skip, @Take",
                    new SqlParameter("@CategoriesId", (object?)categoryId ?? DBNull.Value),
                    new SqlParameter("@NxbId", (object?)nxbId ?? DBNull.Value),
                    new SqlParameter("@StockFilter", stockFilter),
                    new SqlParameter("@Keyword", (object?)Keyword ?? DBNull.Value),
                    new SqlParameter("@Skip", (pageIndex - 1) * pageSize),
                    new SqlParameter("@Take", pageSize))
                .ToListAsync();
            await WriteAdminListCache(key, items);
            return items;
        }

        // ================================================================
        // ADMIN — DETAIL (không cache — cần data mới nhất, có ImportPrice)
        // ================================================================
        public async Task<ProductAdminDetailDto?> GetAdminDetail(Guid id)
        {
            var data = (await _context.Database.SqlQueryRaw<ProductAdminDetailDto>("EXEC sp_GetProductAdminDetail @Id",
                new SqlParameter("@Id", id)).ToListAsync()).FirstOrDefault();
            if (data == null) return null;
            return data;
        }
        public async Task<Product> GetProductDetail(Guid id)
        {
            return await _context.products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<ProductAdminFiltered>> GetAdminFiltered(int pageIndex, int pageSize, string? Keyword = null)
        {
            var items = await _context.Database
                .SqlQueryRaw<ProductAdminFiltered>(
                    "EXEC sp_getFilteredPurchaseProduct  @Keyword, @Skip, @Take",  
                    new SqlParameter("@Keyword", (object?)Keyword ?? DBNull.Value),
                    new SqlParameter("@Skip", (pageIndex - 1) * pageSize),
                    new SqlParameter("@Take", pageSize))
                .ToListAsync();
            return items;
        }

        public async Task<bool> IsValid(string name)
        {
            return await _context.products.AnyAsync(u => u.Name == name);
        }

        public async Task<Product> AddAsync(Product dto)
        {
            
            await _context.products.AddAsync(dto);
            await _context.SaveChangesAsync();
            await Task.WhenAll(
                InvalidateUserListCache(dto.CategoriesId),
                InvalidateAdminListCache(dto.CategoriesId),
                //Xóa cache trang đầu tiên nữa 
                InvalidateAdminListCache(0));
            return dto;
        }

        public async Task<Product> UpdateAsync(Product dto)
        {
            await _context.SaveChangesAsync();
            var tasks = new List<Task>
            {
                InvalidateUserDetailCache(dto.Id),
                InvalidateUserListCache(dto.CategoriesId),
                InvalidateAdminListCache(dto.CategoriesId),
                InvalidateAdminListCache(0)
            };
            await Task.WhenAll(tasks);
            return dto;
        }

        public async Task<bool> Delete(Guid id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null) return false;

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            await Task.WhenAll(
                InvalidateUserDetailCache(id),
                InvalidateUserListCache(product.CategoriesId),
                InvalidateAdminListCache(product.CategoriesId));

            return true;
        }


        // ================================================================
        // PRIVATE HELPERS
        // ================================================================
        private DateTime Expire() => DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));

        private async Task WriteUserDetailCache(string key, ProductUserDetailDto data)
            => await _userDetailCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedUserDetail { Id = ObjectId.GenerateNewId(), CacheKey = key, Data = data, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task WriteUserListCache(string key, List<ProductUserListingDto> items)
            => await _userListCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedUserListing { Id = ObjectId.GenerateNewId(), CacheKey = key, Items = items, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task WriteAdminListCache(string key, List<ProductAdminListingDto> items)
            => await _adminListCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedAdminListing { Id = ObjectId.GenerateNewId(), CacheKey = key, Items = items, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task InvalidateUserDetailCache(Guid id)
            => await _userDetailCache.DeleteOneAsync(x => x.CacheKey == $"user:detail:{id}");
 
        private async Task InvalidateUserListCache(int categoryId)
            => await _userListCache.DeleteManyAsync(
                Builders<CachedUserListing>.Filter.Regex(
                    x => x.CacheKey, new BsonRegularExpression($"^user:list:{categoryId}:")));
 
        private async Task InvalidateAdminListCache(int categoryId)
            => await _adminListCache.DeleteManyAsync(
                Builders<CachedAdminListing>.Filter.Regex(
                    x => x.CacheKey, new BsonRegularExpression($":cat{categoryId}:")));
    }

    public class CachedUserDetail
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public ProductUserDetailDto Data { get; set; } = default!;
        public DateTime ExpireAt { get; set; }
    }

    public class CachedUserListing
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public List<ProductUserListingDto> Items { get; set; } = [];
        public DateTime ExpireAt { get; set; }
    }

    public class CachedAdminListing
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public List<ProductAdminListingDto> Items { get; set; } = [];
        public DateTime ExpireAt { get; set; }
    }
}
