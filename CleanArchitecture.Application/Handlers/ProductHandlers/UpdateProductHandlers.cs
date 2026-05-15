using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.ProductHandlers
{
    public class UpdateProductHandlers : IRequestHandler<UpdateProductCommand, Product>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public UpdateProductHandlers(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Product> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            try
            {
                //var existing = await _repository.IsValid(request.Name);
                var existing  = await _repository.GetProductDetail(request.Id);
                if (existing == null)
                    throw new Exception("Không tồn tại Sản phẩm có Id như yêu cầu");
                existing.UpdateInfo(request.Name, request.Series, request.NxbId, request.CategoriesId,
                    request.Weight, request.ImportPrice, request.Price, request.PublisherPrice, request.Quantity,
                    request.Discount, request.Description, request.ThumbnailIMG);
                var result = await _repository.UpdateAsync(existing);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception( "Lỗi khi update", ex );
            }
        }
    }
}
