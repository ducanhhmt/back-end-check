using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;


namespace CleanArchitecture.Application.Handlers.ProductHandlers
{
    public class GetbyIdProductAdminHandlers : IRequestHandler<GetbyIdProductAdminQueries, ProductAdminDetailDto>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetbyIdProductAdminHandlers(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductAdminDetailDto> Handle(GetbyIdProductAdminQueries request, CancellationToken ct)
        {
            var products = await _repository.GetAdminDetail(request.Id);
            // return _mapper.Map<ProductUserDetailDto>(products);
            return products;
        }
    }
}
