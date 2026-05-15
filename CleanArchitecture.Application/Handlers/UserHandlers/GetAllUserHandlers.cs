using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class GetAllUserHandlers : IRequestHandler<GetAllUserQueries, List<User>>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetAllUserHandlers(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<User>> Handle(GetAllUserQueries request, CancellationToken ct)
        {
            var users = await _repository.GetAll();
            return users;
        }
    }
}

