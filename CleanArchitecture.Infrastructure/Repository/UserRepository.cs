using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<CachedUserLoginDetail> _userLoginCached;
        public UserRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _userLoginCached = database.GetCollection<CachedUserLoginDetail>("userLogincached");
        }

        public async Task<List<User>> GetAll()
        {
            var lstData = await _context.users.ToListAsync();
            return lstData;
        }
        public async Task<(string hash, string salt)?> GetPasswordByUserId(Guid userId)
        {
            var result = await _context.Database
                .SqlQueryRaw<PasswordUser>(
                    "SELECT PasswordHash, PasswordSalt FROM Nguoi_dung WHERE Id = @UserId",
                    new SqlParameter("@UserId", userId))
                .FirstOrDefaultAsync();
            if (result == null) return null;
            return (result.PasswordHash, result.PasswordSalt);
        }

        public async Task<UserDto> GetById(string id)
        {
            var key = $"user:Login:{id}";
            var cached = await _userLoginCached.Find(x => x.CacheKey == key).FirstOrDefaultAsync();
            if (cached != null) return cached.Data;
            var items = (await _context.Database.SqlQueryRaw<UserDto>("EXEC sp_getUserloginInfo @UserId",
                        new SqlParameter("@UserId", id)).ToListAsync()).FirstOrDefault();
            await WriteUserLoginCached(key, items);
            return items;
        }

        public Task<User> GetByAccount(string account)
        {
            return _context.users.FirstOrDefaultAsync(n => n.Account == account);
        }

        public async Task<bool> IsValid(string account)
        {
            return await _context.users.AnyAsync(u => u.Account == account);
        }

        public async Task<User> AddAsync(User data)
        {
            _context.users.Add(data);
            await _context.SaveChangesAsync();
            //await _collection.InsertOneAsync(data); // Thêm mới
            InvalidateUserLoginCache(data.Id);
            return data;
        }

        public async Task<bool> UpdateAsync(User data)
        {
            _context.users.Update(data);
            await _context.SaveChangesAsync();
            //await _collection.ReplaceOneAsync(x => x.Id == data.Id, data);
            InvalidateUserLoginCache(data.Id);
            return true;
        }

        public async Task<bool> UpdateUserProfile(Guid id, string firstName, string lastName, string email, string phone, bool gender, DateTime birthday)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId",    id),
                    new SqlParameter("@firstName", firstName),
                    new SqlParameter("@lastName",  lastName),
                    new SqlParameter("@email",     email),
                    new SqlParameter("@phone",     phone),
                    new SqlParameter("@gender",    gender),
                    new SqlParameter("@birthday",  birthday)
                };
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_changeUserProfile @UserId, @firstName, @lastName, @email, @phone, @gender, @birthday", parameters
                );
                InvalidateUserLoginCache(id);
                return true;
            }
            catch (SqlException ex)
            {
                // Lỗi từ SQL Server (sai kiểu dữ liệu, constraint, timeout,...)
                Console.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Lỗi khác (connection, null reference,...)
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateUserAddress(Guid id, string Address)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId",    id),
                    new SqlParameter("@Address", Address)
                };
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_changeUserAddress @UserId, @Address", parameters
                );
                InvalidateUserLoginCache(id);
                return true;
            }
            catch (SqlException ex)
            {
                // Lỗi từ SQL Server (sai kiểu dữ liệu, constraint, timeout,...)
                Console.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Lỗi khác (connection, null reference,...)
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangePassword(Guid id, string passwordHash, string passwordSalt)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserId",    id),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@PasswordSalt",  passwordSalt)
                };
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_changeUserPassword @UserId, @PasswordHash, @PasswordSalt", parameters
                );
                InvalidateUserLoginCache(id);
                return true;
            }
            catch (SqlException ex)
            {
                // Lỗi từ SQL Server (sai kiểu dữ liệu, constraint, timeout,...)
                Console.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Lỗi khác (connection, null reference,...)
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(string id)
        {
            var data = await _context.users.FirstOrDefaultAsync(n => n.Id.ToString() == id);
            _context.users.Remove(data);
            await _context.SaveChangesAsync();
            //await _collection.DeleteOneAsync(x => x.Id.ToString() == id);
            InvalidateUserLoginCache(Guid.Parse(id));
            return true;
        }

        public async Task<bool> Remove(User data)
        {
            _context.users.Remove(data);
            await _context.SaveChangesAsync();
            //await _collection.DeleteOneAsync(x => x.Id.ToString() == data.Id.ToString());
            InvalidateUserLoginCache(data.Id);
            return true;
        }

        private DateTime Expire() => DateTime.UtcNow.Add(TimeSpan.FromMinutes(10));

        private async Task WriteUserLoginCached(string key, UserDto data)
            => await _userLoginCached.ReplaceOneAsync(
                x => x.CacheKey == key,
                new CachedUserLoginDetail { Id = ObjectId.GenerateNewId(), CacheKey = key, Data = data, ExpireAt = Expire() },
                new ReplaceOptions { IsUpsert = true });

        private async Task InvalidateUserLoginCache(Guid id)
            => await _userLoginCached.DeleteOneAsync(x => x.CacheKey == $"user:Login:{id}");
    }

    public class CachedUserLoginDetail
    {
        public ObjectId Id { get; set; }
        public string CacheKey { get; set; } = default!;
        public UserDto Data { get; set; } = default!;
        public DateTime ExpireAt { get; set; }
    }
    public class PasswordUser
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
    }

}
