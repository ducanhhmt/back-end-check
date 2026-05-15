using AutoMapper;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class ChangeUserPasswordHandlers : IRequestHandler<ChangeUserPasswordCommand, bool>
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _hasher;
        public ChangeUserPasswordHandlers(IUserRepository repository, IPasswordHasher hasher)
        {
            _repository = repository;
            _hasher = hasher;
        }

        public async Task<bool> Handle(ChangeUserPasswordCommand request, CancellationToken ct)
        {
            var stored = await _repository.GetPasswordByUserId(request.UserId);
            bool valid = _hasher.Verify(request.currentPassword, stored.Value.hash, stored.Value.salt);
            if (!valid)
                throw new UnauthorizedAccessException();
            var (hash, salt) = _hasher.HashPassword(request.newPassword);
            var result = await _repository.ChangePassword(request.UserId, hash, salt);
            return result;
        }

    }
}

