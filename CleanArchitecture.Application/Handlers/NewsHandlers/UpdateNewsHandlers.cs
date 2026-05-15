using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class UpdateNewsHandlers : IRequestHandler<UpdateNewsCommand, bool>
    {
        private readonly INewsRepository _repository;
        private readonly IMapper _mapper;

        public UpdateNewsHandlers(INewsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateNewsCommand request, CancellationToken ct)
        {
            var isSuccess = false;
            var existing = await _repository.IsValid(request.Id);
            if (!existing)
                return isSuccess;
            isSuccess = await _repository.UpdateAsync(_mapper.Map<News>(request));
            return isSuccess;
        }
    }
}
