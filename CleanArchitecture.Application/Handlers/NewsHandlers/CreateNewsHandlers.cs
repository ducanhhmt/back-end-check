using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class CreateNewsHandlers : IRequestHandler<AddNewsCommand, bool>
    {
        private readonly INewsRepository _repository;
        private readonly IMapper _mapper;

        public CreateNewsHandlers(INewsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(AddNewsCommand request, CancellationToken ct)
        {
            var isSuccess = false;
            var data = _mapper.Map<News>(request);
            isSuccess = await _repository.AddAsync(data);
            return isSuccess;
        }
    }
}
