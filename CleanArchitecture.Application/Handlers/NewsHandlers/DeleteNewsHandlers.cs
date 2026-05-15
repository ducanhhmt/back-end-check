using AutoMapper;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class DeleteNewsHandlers : IRequestHandler<DeleteNewsCommand, bool>
    {
        private readonly INewsRepository _repository;
        private readonly IMapper _mapper;

        public DeleteNewsHandlers(INewsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(DeleteNewsCommand request, CancellationToken ct)
        {
            return await _repository.Delete(request.Id);
        }
    }
}
