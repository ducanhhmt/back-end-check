using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Purchase
{
    public class UpdatePurchaseCommand : IRequest<bool> {
        public Guid Id { get; set; }
        public string UserCreated { get; set; }
        public int SupplierId { get; set; }
        public int ImportPrice { get; set; }
        public List<PurchaseOrder> PurchaseItems { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public int State { get; set; }
    }
}
