using CleanArchitecture.Application.Model;
using MediatR;


namespace CleanArchitecture.Application.Features.Purchase
{
    public class GetInfoPurchaseQueries : IRequest<PurchaseDetailModel> 
    {
        public Guid Id { get; set; }
    }

}
