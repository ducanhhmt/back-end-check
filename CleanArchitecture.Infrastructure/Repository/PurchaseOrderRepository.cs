using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CleanArchitecture.Infrastructure.Repository
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly DataContext _context;
        public PurchaseOrderRepository(DataContext context, IMongoDatabase database)
        {
            _context = context;
        }
        public async Task<List<PurchaseOrderDetailModel>> getPurchaseOrder(Guid purchaseId)
        {
              var data = (await _context.Database.SqlQueryRaw<PurchaseOrderDetailModel>("EXEC sp_GetPurchaseOrdersByPurchaseId @PurchaseId",
                        new SqlParameter("@PurchaseId", purchaseId)).ToListAsync());
                if (data == null) return null;
                return data;
        }


        public async Task<bool> IsValid(string name)
        {
            return await _context.products.AnyAsync(u => u.Name == name);
        }

        public async Task<bool> Add(PurchaseOrder data)
        {    
            _context.purchaseOrder.Add(data);
            await _context.SaveChangesAsync();
            //await Task.WhenAll(
            //    InvalidateUserListCache(product.CategoriesId),
            //    InvalidateAdminListCache(product.CategoriesId));
            return true;
        }
    }
}
