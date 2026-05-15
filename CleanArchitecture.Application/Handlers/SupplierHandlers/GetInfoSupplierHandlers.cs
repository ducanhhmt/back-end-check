using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Supplier;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.SupplierHandlers
{
    public class GetInfoSupplierHandlers : IRequestHandler<GetInfoSupplierQueries, SupplierInfoModel>
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public GetInfoSupplierHandlers(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SupplierInfoModel> Handle(GetInfoSupplierQueries request, CancellationToken ct)
        {
            var data  = await _repository.GetInfoModel(request.Id);
            return data;
        }
    }
}
