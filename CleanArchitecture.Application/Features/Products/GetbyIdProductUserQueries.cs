using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Products
{
    public class GetbyIdProductUserQueries : IRequest<ProductUserDetailDto> 
    {
        public Guid Id { get; set; }
    }
}
