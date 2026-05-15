using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Products
{
    public class GetProductAdminListingQueries : IRequest<ProductAdminFilterRespone> {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int? NxbId { get; set; }
        public string? Stock {  get; set; }
        public int Pagesize { get; set; }
        public int PageIndex { get; set; }

    }
}
