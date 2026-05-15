using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class BillInfoRepository : IBillInfoRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<BillInfo> _collection;
        public BillInfoRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _collection = database.GetCollection<BillInfo>("BillInfo");
        }
        
        public async Task<List<BillInfo>> GetAll()
        {
            return new List<BillInfo>();
        }       

        public async Task<List<BillInfo>> GetById(Guid billId)
        {
            var cached = await _collection.Find(x => x.BillId == billId).ToListAsync();
            if (cached != null)
            {
                return cached;
            }
            return await _context.billInfo.Where(n => n.BillId == billId).ToListAsync();
        }

        public async Task<BillInfo> AddAsync(BillInfo data)
        {
            _context.billInfo.Add(data);
            await _context.SaveChangesAsync();
            await _collection.InsertOneAsync(data); // Thêm mới
            return data;
        }       
       
        public async Task<bool> Remove(BillInfo data)
        {
            _context.billInfo.Remove(data);
            await _context.SaveChangesAsync();
            await _collection.DeleteOneAsync(x => x.Id.ToString() == data.Id.ToString());
            return true;
        }

        public async Task<bool> RemoveByBillId(Guid billId)
        {
            var param = new SqlParameter("@BillId", billId);
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteBillInfo @BillId", param );
            // Xóa cache Mongo
            await _collection.DeleteManyAsync(x => x.BillId == billId);
            return true;
        }
    }
}
