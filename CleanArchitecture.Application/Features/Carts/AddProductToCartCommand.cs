using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Carts
{
    public class AddProductToCartCommand : IRequest<CartDTO>
    {
        public Guid ProductId { get; set; }
        public Guid? UserId { get; set; }
        public int Quantity { get; set; }
    }
}
