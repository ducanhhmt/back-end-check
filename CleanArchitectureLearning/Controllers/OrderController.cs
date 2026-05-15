using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Bill;
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
    public class OrderController : ControllerBase
    {
        private readonly ISender _sender;
        public OrderController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("GetAllUserBillInfo")]
        [ProducesResponseType(typeof(UserBillRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserBillRespone>> GetAll([FromBody] GetAllUserBillInfoQueries command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.userId = Guid.Parse(userId);
            var result = await _sender.Send(command);
            return result;
        }

        [HttpGet("GetBillInfo")]
        [ProducesResponseType(typeof(UserBillDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<UserBillDTO> GetBillInfo(Guid billId)
        {
            var result = await _sender.Send(new GetBillInfoQueries { BillId = billId });
            return result;
        }

        [HttpGet("UserBillStateSummary")]
        [ProducesResponseType(typeof(BillStateSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetStateSummary()
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            var result = await _sender.Send(new GetBillStateSummaryQueries { userId = Guid.Parse(userId) });
            return Ok(result);
        }

        [HttpPost("CreateBill")]
        [ProducesResponseType(typeof(UserBillDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserBillDTO>> Create([FromBody] CreateBillCommand command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.userId = Guid.Parse(userId);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpDelete("RemoveBill")]
        public async Task<IActionResult> Delete([FromBody] RemoveBillCommand command)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            command.userId = Guid.Parse(userId);
            var result = await _sender.Send(command);
            return result ? Ok(result) : NotFound();
        }
    }
}
