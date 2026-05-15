using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Event;

namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class CreateUserHandlers : IRequestHandler<AddUserCommand, User>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _hasher;
        private readonly IPublisher _publisher;
        public CreateUserHandlers(IUserRepository repository, IMapper mapper, IPasswordHasher hasher, IPublisher publisher)
        {
            _repository = repository;
            _mapper = mapper;
            _hasher = hasher;
            _publisher = publisher;
        }

        public async Task<User> Handle(AddUserCommand request, CancellationToken ct)
        {
            var (hash, salt) = _hasher.HashPassword(request.Password);
            var data = new User("", "", "", "", "", request.Account, hash, salt, "User", null, "");
            var newData = await _repository.AddAsync(data);
            return newData;
        }
    }
}

