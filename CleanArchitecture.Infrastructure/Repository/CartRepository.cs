using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<CachedUserCart> _usercartCache;
        public CartRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _usercartCache = database.GetCollection<CachedUserCart>("cache_userCart");
        }

        public async Task<List<Cart>> GetAll()
        {
            var lstData = await _context.carts.ToListAsync();
            return lstData;
        }

        public async Task<Cart> GetById(int id)
        {
            return await _context.carts.FirstOrDefaultAsync(n => n.Id == id);
        }



        public async Task<List<UserCartModel>> GetCartByUserId(Guid userId)
        {
            var key = $"userCart:Id:{userId}";
            var cached = await _usercartCache.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null) return cached.Items;
            var items = await _context.Database
                .SqlQueryRaw<UserCartModel>(
                    "EXEC sp_GetUserCart @UserId",
                    new SqlParameter("@UserId", (object?)userId ?? DBNull.Value))
                .ToListAsync();
            await WriteUserCartCache(key, items);
            return items;
        }
        public Task<Cart> checkProductExitsOnCart(Guid userId, Guid productId)
        {
            return _context.carts.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);
        }
        public async Task<Cart> Add(Cart data)
        {
            _context.carts.Add(data);
            await _context.SaveChangesAsync();
            await InvalidateUserCartCache(data.UserId); // Thêm mới
            return data;
        }
        public async Task<Cart> Update(Cart data)
        {
            _context.carts.Update(data);
            await _context.SaveChangesAsync();
            await InvalidateUserCartCache(data.UserId);
            return data;
        }
        public async Task<bool> Remove(RemoveCartbyUserId request)
        {
            var data = await _context.carts.FirstOrDefaultAsync(n => n.ProductId == request.productId && n.UserId == request.userId);
            _context.carts.Remove(data);
            await _context.SaveChangesAsync();
            await InvalidateUserCartCache(request.userId);
            return true;
        }

        public async Task<bool> RemoveCartOnUser(Guid userId, Guid ProductId)
        {
            var billParam = new SqlParameter("@UserId", userId);
            var productParam = new SqlParameter("@ProductId", ProductId);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeleteCartProductOnUser @UserId, @ProductId",
                billParam,
                productParam
            );
            // Xóa cache Mongo
            await InvalidateUserCartCache(userId);
            return true;
        }

        private DateTime Expire() => DateTime.UtcNow.Add(TimeSpan.FromMinutes(10));

        private async Task WriteUserCartCache(string key, List<UserCartModel> data)
            => await _usercartCache.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedUserCart { Id = ObjectId.GenerateNewId(), CacheKey = key, Items = data, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task InvalidateUserCartCache(Guid id)
            => await _usercartCache.DeleteOneAsync(x => x.CacheKey == $"userCart:Id:{id}");
    }
    public class CachedUserCart
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public List<UserCartModel> Items { get; set; } = [];
        public DateTime ExpireAt { get; set; }
    }
}
