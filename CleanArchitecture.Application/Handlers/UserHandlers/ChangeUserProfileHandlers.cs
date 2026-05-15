using AutoMapper;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class ChangeUserProfileHandlers : IRequestHandler<ChangeUserProfileCommand, bool>
    {
        private readonly IUserRepository _repository;

        public ChangeUserProfileHandlers(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ChangeUserProfileCommand request, CancellationToken ct)
        {
            var result = await _repository.UpdateUserProfile(request.Id, request.firstName, request.lastName, request.email,
                request.phone, request.gender, request.birthday);
            return result;
        }

    }
}

