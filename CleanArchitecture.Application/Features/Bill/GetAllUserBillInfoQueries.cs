using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Model;
using MediatR;

namespace CleanArchitecture.Application.Features.Bill
{
    public class GetAllUserBillInfoQueries : IRequest<UserBillRespone>
    {
        public Guid? userId { get; set; }
        public int pagesize { get; set; }
        public int pageIndex { get; set; }
        public int? state { get; set; }
    }
}
