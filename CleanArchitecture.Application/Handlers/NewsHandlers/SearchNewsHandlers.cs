using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class SearchNewsHandlers : IRequestHandler<SearchingNewsQueries, List<NewsDTO>>
    {
        private readonly INewsRepository _repository;
        private readonly IMapper _mapper;

        public SearchNewsHandlers(INewsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<NewsDTO>> Handle(SearchingNewsQueries request, CancellationToken ct)
        {
            var data = await _repository.Search(request.Keyword);
            return _mapper.Map<List<NewsDTO>>(data);
        }
    }
}
