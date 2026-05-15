using CleanArchitecture.Application.Model;
using MediatR;


namespace CleanArchitecture.Application.Features.Supplier
{
    public class GetInfoSupplierQueries : IRequest<SupplierInfoModel> 
    {
        public int Id { get; set; }
    }

}
