using AutoMapper;
using CleanArchitecture.Application.DTO;
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

namespace CleanArchitecture.Application.Handlers.ProductHandlers
{
    public class CreateProductsHandlers : IRequestHandler<AddProductCommand, Product>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public CreateProductsHandlers(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Product> Handle(AddProductCommand request, CancellationToken ct)
        {
            var finalData = new Product(request.Name, request.Series, request.NxbId, request.CategoriesId, request.Weight,
                request.ImportPrice, request.Price, request.PublisherPrice,request.Quantity, request.Quantity, request.Discount, request.Description, request.ThumbnailIMG);
            var result = await _repository.AddAsync(finalData);
            return result;
        }
    }
}
