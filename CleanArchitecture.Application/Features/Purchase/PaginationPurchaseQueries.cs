using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using MediatR;

namespace CleanArchitecture.Application.Features.Purchase
{
    public class PaginationPurchaseQueries : IRequest<PurchasePaginationRespone> {
        public string? Keyword { get; set; }
        public int Pagesize { get; set; }
        public int PageIndex { get; set; }
    }
}
