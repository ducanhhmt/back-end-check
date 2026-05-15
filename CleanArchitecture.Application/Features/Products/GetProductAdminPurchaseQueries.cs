using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Products
{
    public class GetProductAdminPurchaseQueries : IRequest<List<ProductAdminFiltered>> {
        public string? Keyword { get; set; }
        public int Pagesize { get; set; }
        public int PageIndex { get; set; }
    }
}
