using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Carts;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Handlers.NewsHandlers
{
    public class AddProductToCartHandlers : IRequestHandler<AddProductToCartCommand, CartDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public AddProductToCartHandlers(IProductRepository productRepository, ICartRepository cartRepository ,IMapper mapper)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO> Handle(AddProductToCartCommand request, CancellationToken ct)
        {
            var checkExits = await _cartRepository.checkProductExitsOnCart(request.UserId.Value, request.ProductId);
            if (checkExits != null)
            {
                // Đã tồn tại -> update quantity
                checkExits.UpdateInfo(
                 checkExits.ProductId, checkExits.UserId, checkExits.Name,
                 checkExits.Price, checkExits.PublisherPrice,
                 checkExits.Quantity + request.Quantity,  // cộng quantity
                checkExits.Image);
                var updated = await _cartRepository.Update(checkExits);
                return _mapper.Map<CartDTO>(updated);
            }
            else
            {
                var productInfo = await _productRepository.GetAdminDetail(request.ProductId);
                Cart cartData = new Cart(
                    productInfo.Id, request.UserId.Value, productInfo.Name,
                    productInfo.Price, productInfo.PublisherPrice, request.Quantity, productInfo.Images[0]);
                var dataRespone = await _cartRepository.Add(cartData);
                return _mapper.Map<CartDTO>(dataRespone);
            }           
        }
    }
}
