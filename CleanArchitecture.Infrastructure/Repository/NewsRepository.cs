using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly DataContext _context;
        private readonly IMongoCollection<News> _collection;
        public NewsRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
            _collection = database.GetCollection<News>("News");
        }

        public async Task<List<News>> GetAll()
        {
            var cached = await _collection.Find(_ => true).ToListAsync();
            if (cached != null && cached.Count > 0)
            {
                return cached;
            }
            var lstData = await _context.news.ToListAsync();
            await _collection.InsertManyAsync(lstData);
            return lstData;
        }

        public async Task<List<News>> Search(string keyword)
        {
            // Dùng Regex để ép quy chuẩn text gần giống trong Mongo
            var filter = Builders<News>.Filter.Regex( x => x.Title, new BsonRegularExpression(keyword, "i"));// "i" = case-insensitive 
            // search trong Mongo trước
            var cached = await _collection.Find(filter).ToListAsync();
            if (cached != null && cached.Count > 0)
                return cached;
            // fallback DB
            var query = await _context.news.Where(x => x.Title.Contains(keyword)).ToListAsync();
            if (query.Any())
                await _collection.InsertManyAsync(query);
            return query;
        }

        public Task<News> GetById(string id)
        {
            var cached = _collection.Find(x => x.Id.ToString() == id).FirstOrDefaultAsync();
            if (cached != null)
            {
                return cached;
            }
            return _context.news.FirstOrDefaultAsync(n => n.Id.ToString() == id);
        }

        public async Task<bool> AddAsync(News data)
        {
            _context.news.Add(data);
            await _context.SaveChangesAsync();
            await _collection.InsertOneAsync(data); // Thêm mới
            return true;
        }
      
        public async Task<bool> UpdateAsync(News news)
        {
            _context.news.Update(news);
            await _context.SaveChangesAsync();
            await _collection.ReplaceOneAsync(x => x.Id == news.Id, news);
            return true;
        }

        public async Task<bool> Delete(string id)
        {
            var data = await _context.news.FirstOrDefaultAsync(n => n.Id.ToString() == id);
            _context.news.Remove(data);
             await _context.SaveChangesAsync();
            await _collection.DeleteOneAsync(x => x.Id.ToString() == id);
            return true;
        }

        public async Task<bool> IsValid(string id)
        {
            return await _context.news.AnyAsync(u => u.Id.ToString() == id);
        }
    }
}
