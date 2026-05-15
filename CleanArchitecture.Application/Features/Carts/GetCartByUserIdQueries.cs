using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Carts
{
    public class GetCartByUserIdQueries : IRequest<List<UserCartModel>>
    {
        public string Id { get; set; }
    }
}
