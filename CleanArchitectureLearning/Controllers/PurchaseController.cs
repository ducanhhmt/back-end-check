using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Features.Purchase;
using CleanArchitecture.Application.Features.Products;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PurchaseController : ControllerBase
    {
        private readonly ISender _sender;
        public PurchaseController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<PurchasePaginationRespone> PaginationFilter([FromBody] PaginationPurchaseQueries request)
        {
            return await _sender.Send(request);
        }

        [HttpGet]
        public async Task<PurchaseDetailModel>GetInfoPurchase(Guid id)
        {
            return await _sender.Send(new GetInfoPurchaseQueries { Id = id });
        }

        [HttpPost, Route("Add")]
        public async Task<bool> Add([FromBody] AddNewPurchaseCommand request)
        {
            return await _sender.Send(request);
        }
        [HttpPut, Route("Update")]
        public async Task<bool> Update([FromBody] UpdatePurchaseCommand request)
        {
            return await _sender.Send(request);
        }
    }
}
