using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Supplier;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.SupplierHandlers
{
    public class GetAllSupplierHandlers : IRequestHandler<GetAllSupplierQueries, List<SupplierViewModel>>
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSupplierHandlers(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SupplierViewModel>> Handle(GetAllSupplierQueries request, CancellationToken ct)
        {
            var data  = await _repository.GetAll();
            return data;
        }
    }
}
