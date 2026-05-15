using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;


namespace CleanArchitecture.Application.Handlers.ProductHandlers
{
    public class GetbyIdProductUserHandlers : IRequestHandler<GetbyIdProductUserQueries, ProductUserDetailDto>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetbyIdProductUserHandlers(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductUserDetailDto> Handle(GetbyIdProductUserQueries request, CancellationToken ct)
        {
            var products = await _repository.GetUserDetail(request.Id);
            // return _mapper.Map<ProductUserDetailDto>(products);
            return products;
        }
    }
}
