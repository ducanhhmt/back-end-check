using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;


namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class GetbyIdNewsHandlers : IRequestHandler<GetbyIdNewsQueries, NewsDTO>
    {
        private readonly INewsRepository _repository;
        private readonly IMapper _mapper;

        public GetbyIdNewsHandlers(INewsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NewsDTO> Handle(GetbyIdNewsQueries request, CancellationToken ct)
        {
            var products = await _repository.GetById(request.Id);
            return _mapper.Map<NewsDTO>(products);
        }
    }
}
