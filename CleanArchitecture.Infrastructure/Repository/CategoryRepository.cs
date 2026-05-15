using CleanArchitecture.Application.Cache;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        private readonly IMemCacheData _cacheData;
        public CategoryRepository(DataContext context, IMemCacheData cacheData)
        {
            _context = context;
            _cacheData = cacheData;
        }
        public async Task<List<Category>> GetAll()
        {
            return new List<Category>();
        }
        public List<Category> GetAllCategories()
        {
            var cached = _cacheData.Get<List<Category>>(CacheKey.Category);
            if (cached != null && cached.Count > 0)
            {
                return cached;
            }
            var lstData = _context.categories.ToList();
            _cacheData.Set<List<Category>>(CacheKey.Category, lstData, TimeSpan.FromDays(30));
            return lstData;
        }       
    }
}
