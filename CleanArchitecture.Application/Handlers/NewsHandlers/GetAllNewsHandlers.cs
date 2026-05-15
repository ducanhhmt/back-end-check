using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class GetAllNewsHandlers : IRequestHandler<GetAllNewsQueries, List<NewsDTO>>
    {
        private readonly INewsRepository _repository;
        private readonly IMapper _mapper;

        public GetAllNewsHandlers(INewsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<NewsDTO>> Handle(GetAllNewsQueries request, CancellationToken ct)
        {
            var products = await _repository.GetAll();
            return _mapper.Map<List<NewsDTO>>(products);
        }
    }
}
