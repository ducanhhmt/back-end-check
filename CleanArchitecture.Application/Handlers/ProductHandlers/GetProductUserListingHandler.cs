using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.ProductHandlers
{
    public class GetProductUserListingHandler : IRequestHandler<GetProductUserListingQueries, ProductUserFilterRespone>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductUserListingHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        /// <summary>
        ///  Xử lý lọc phân trang của admin
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<ProductUserFilterRespone> Handle(GetProductUserListingQueries request, CancellationToken ct)
        {
            var queryData = await _repository.GetUserListing(request.Keyword, request.CategoryId, request.PageIndex, request.Pagesize, request.Stock, request.NxbId, request.minPrice, request.maxPrice);
            if (queryData != null && queryData.Count > 0)
            {
                return new ProductUserFilterRespone
                {
                    products = queryData,
                    TotalRecords = queryData.FirstOrDefault()?.TotalRecords ?? 0,
                    pageCount = queryData.FirstOrDefault()?.TotalPages ?? 0,
                };
            }
            return new ProductUserFilterRespone();
        }
    }
}
