using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        private readonly ISender _sender;
        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<List<ProductDTO>> GetAll()
        {
            var result = await _sender.Send(new GetAllProductQueries());
            return result;
        }

        [HttpGet("user-search/{id}")]
        [ProducesResponseType(typeof(ProductUserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ProductUserDetailDto> GetByIdUser(Guid id)
        {
            var product = await _sender.Send(new GetbyIdProductUserQueries { Id = id });
            return product;
        }

        [HttpGet("admin-search/{id}")]
        [ProducesResponseType(typeof(ProductAdminDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ProductAdminDetailDto> GetByIdAdmin(Guid id)
        {
            var product = await _sender.Send(new GetbyIdProductAdminQueries { Id = id });
            return product;
        }

        [HttpPost("AdminFilter")]
        [ProducesResponseType(typeof(ProductAdminFilterRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ProductAdminFilterRespone> ProductAdminFilter([FromBody] GetProductAdminListingQueries command)
        {
            var product = await _sender.Send(command);
            return product;
        }
        [HttpPost("AdminPurchaseFilter")]
        [ProducesResponseType(typeof(List<ProductAdminFiltered>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<List<ProductAdminFiltered>> ProductAdminPurchaseFilter([FromBody] GetProductAdminPurchaseQueries command)
        {
            var product = await _sender.Send(command);
            return product;
        }

        [HttpPost("UserFilter")]
        [ProducesResponseType(typeof(ProductUserFilterRespone), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ProductUserFilterRespone> ProductUserFilter([FromBody] GetProductUserListingQueries command)
        {
            var product = await _sender.Send(command);
            return product;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Product> Create([FromBody] AddProductCommand command)
        {
            var result = await _sender.Send(command);
            return result;
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Product> Update([FromBody] UpdateProductCommand command)
        {
            var result = await _sender.Send(command);
            return result;
        }
    }
}
