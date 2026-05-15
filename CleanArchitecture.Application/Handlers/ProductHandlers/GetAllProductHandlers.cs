using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.ProductHandlers
{
    public class GetAllProductHandlers : IRequestHandler<GetAllProductQueries, List<ProductDTO>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetAllProductHandlers(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> Handle(GetAllProductQueries request, CancellationToken ct)
        {
            var products = await _repository.GetAll();
            return _mapper.Map<List<ProductDTO>>(products);
        }
    }
}
