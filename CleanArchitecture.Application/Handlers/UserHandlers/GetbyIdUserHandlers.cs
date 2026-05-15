using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;


namespace CleanArchitecture.Application.Handlers.UserHandlers
{
    public class GetbyIdUserHandlers : IRequestHandler<GetByIdUserQueries, UserLoginModel>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetbyIdUserHandlers(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserLoginModel> Handle(GetByIdUserQueries request, CancellationToken ct)
        {
            var user = await _repository.GetById(request.UserId.ToString());

            return _mapper.Map<UserLoginModel>(user);
        }
    }
}
