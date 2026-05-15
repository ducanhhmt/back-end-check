using AutoMapper;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class UpdateUserAddressHandlers : IRequestHandler<UpdateUserAddressCommand, bool>
    {
        private readonly IUserRepository _repository;
        public UpdateUserAddressHandlers(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateUserAddressCommand request, CancellationToken ct)
        {
            var result = await _repository.UpdateUserAddress(request.UserId, request.Address);
            return result;
        }

    }
}

