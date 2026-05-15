using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using MediatR;

namespace CleanArchitecture.Application.Features.Bill
{
    public class GetBillStateSummaryQueries : IRequest<BillStateSummary>
    {
        public Guid userId { get; set; }
    }
}
