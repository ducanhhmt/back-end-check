using CleanArchitecture.Application.DTO;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Products
{
    public class GetAllProductQueries : IRequest<List<ProductDTO>> {}
}
