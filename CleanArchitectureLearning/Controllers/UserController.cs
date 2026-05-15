using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CleanArchitecture.Application.Model;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;
        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<List<User>> GetAll()
        {
            var result = await _sender.Send(new GetAllUserQueries());
            return result;
        }

        [HttpGet("UserInfo")]
        public async Task<ActionResult<UserLoginModel>> GetById()
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            var result = await _sender.Send(new GetByIdUserQueries { UserId = Guid.Parse(userId) });
            return result;
        }

        [HttpPost]
        public async Task<User> Add(string account, string password)
        {
            return await _sender.Send(new AddUserCommand{ Account = account, Password = password});
        }

        [HttpPut, Route("updatePassword")]
        public async Task<ActionResult<bool>> UpdatePassword([FromBody] ChangeUserPasswordCommand command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.UserId = Guid.Parse(userId);
            return await _sender.Send(command);
        }

        [HttpPut, Route("updateUserProfile")]
        public async Task<ActionResult<bool>> UpdateUserProfile([FromBody] ChangeUserProfileCommand command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.Id = Guid.Parse(userId);
            return await _sender.Send(command);
        }

        [HttpPut, Route("changeUserAddress")]
        public async Task<ActionResult<bool>> ChangeAddress([FromBody] UpdateUserAddressCommand command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.UserId = Guid.Parse(userId);
            return await _sender.Send(command);
        }

        [HttpPost, Route("Login")]
        public async Task<LoginRespone> Login([FromBody] LoginModel request)
        {
            return await _sender.Send(new UserLoginQueries { Account = request.Account, Password = request.Password });
        }

        public class LoginModel
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
    }
}
