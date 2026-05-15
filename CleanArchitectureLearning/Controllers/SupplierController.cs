using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Features.Supplier;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class SupplierController : ControllerBase
    {
        private readonly ISender _sender;
        public SupplierController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<List<SupplierViewModel>> GetAll()
        {
            var result = await _sender.Send(new GetAllSupplierQueries());
            return result;
        }

        [HttpGet("SupplierInfo/{id}")]
        public async Task<ActionResult<SupplierInfoModel>> GetById(int id)
        {
            var userId = User.FindFirstValue("Id");
            if (userId == null) return Unauthorized("Chưa đăng nhập");
            var result = await _sender.Send(new GetInfoSupplierQueries { Id = id });
            return result;
        } 
    }
}
