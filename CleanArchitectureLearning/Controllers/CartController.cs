using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Carts;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CartController : ControllerBase
    {
        private readonly ISender _sender;
        public CartController(ISender sender)
        {
            _sender = sender;
        }


        [HttpGet("Cart")]
        public async Task<ActionResult<List<UserCartModel>>> GetCartByUserId()
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            var result = await _sender.Send(new GetCartByUserIdQueries { Id = userId });
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CartDTO>> Create([FromBody] AddProductToCartCommand command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.UserId = Guid.Parse(userId);
            var result = await _sender.Send(command);
            return result;
        }
    }
}
