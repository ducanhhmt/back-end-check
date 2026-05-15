using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Products
{
    public class GetProductUserListingQueries : IRequest<ProductUserFilterRespone> {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int? NxbId { get; set; }
        public string? Stock {  get; set; }
        public int? minPrice { get; set; }
        public int? maxPrice { get; set; }
        public int Pagesize { get; set; }
        public int PageIndex { get; set; }

    }
}
