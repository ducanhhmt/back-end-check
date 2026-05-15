using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Carts;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.CartHandlers
{
    public class GetCartByUserIdHandlers : IRequestHandler<GetCartByUserIdQueries, List<UserCartModel>>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public GetCartByUserIdHandlers(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<UserCartModel>> Handle(GetCartByUserIdQueries request, CancellationToken ct)
        {
            var data = await _repository.GetCartByUserId(Guid.Parse(request.Id));
            return data;
        }
    }
} 
