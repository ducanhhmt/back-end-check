using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CategoryController : ControllerBase
    {
        private readonly ISender _sender;
        public CategoryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<List<Category>> GetAll()
        {
            var result = await _sender.Send(new GetAllCategoryQueries());
            return result;
        }
        [HttpGet("Menu")]
        public async Task<List<MenuItem>> GetMenu()
        {
            var result = await _sender.Send(new GetMenuCategoryQueries());
            return result;
        }
    }
}
