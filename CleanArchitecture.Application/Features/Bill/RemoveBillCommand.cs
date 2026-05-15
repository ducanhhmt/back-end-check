using MediatR;

namespace CleanArchitecture.Application.Features.Bill
{
    public class RemoveBillCommand : IRequest<bool>
    {
        public Guid? userId { get; set; }
        public Guid BillId { get; set; }
    }
}
