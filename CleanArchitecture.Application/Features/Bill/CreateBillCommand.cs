using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Bill
{
    public class CreateBillCommand : IRequest<UserBillDTO>
    {
        public Guid? userId { get; set; }
        public string userName { get; set; }
        public string Address { get; set; }
        public string Phone {  get; set; }
        public int ShippingPrice { get; set; }
        public int DiscountPrice  { get; set; }
        public int TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = "cod"; // ← thêm mới
        public List<BillInfoRequest> Items { get; set; }
    }
    public class BillInfoRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
