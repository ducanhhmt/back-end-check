using AutoMapper;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class UserLoginHandlers : IRequestHandler<UserLoginQueries, LoginRespone>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public UserLoginHandlers(IUserRepository repository, IMapper mapper , IPasswordHasher hasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _hasher = hasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginRespone> Handle(UserLoginQueries request, CancellationToken ct)
        {
            var user = await _repository.GetByAccount(request.Account);
            if (user == null)
            {
                return new LoginRespone()
                {
                    isLogin = false,
                    Token = string.Empty,
                    Name = string.Empty
                };
            }
            bool valid = _hasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!valid)
                throw new UnauthorizedAccessException();
            var token = await _jwtTokenGenerator.Generate(user.Id.ToString(), (user.FirstName + "" + user.LastName).ToString(), user.Role);
            return new LoginRespone()
            {
                isLogin = true,
                Token = token,
                Name = user.FirstName + " " + user.LastName,
                Role = user.Role
            };
        }

    }
}

