using CleanArchitecture.Application.DTO;
using MediatR;

namespace CleanArchitecture.Application.Features.Bill
{
    public class GetBillInfoQueries : IRequest<UserBillDTO>
    {
        public Guid BillId { get; set; }
    }
}
