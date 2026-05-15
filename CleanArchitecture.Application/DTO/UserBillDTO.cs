using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTO
{
    public class UserBillDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PayUrl { get; set; } = null!; // ← thêm mới, null nếu COD
        public string UserName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int TotalPrice { get; set; }
        public int State { get; set; }
        public int ShippingPrice { get; set; }
        public int DiscountPrice { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public List<BillInfo> Items { get; set; }
    }
}
