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
    public class GetProductAdminPurchaseFilteredHandler : IRequestHandler<GetProductAdminPurchaseQueries, List<ProductAdminFiltered>>
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public GetProductAdminPurchaseFilteredHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        /// <summary>
        ///  Xử lý filter nhập kho
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<List<ProductAdminFiltered>> Handle(GetProductAdminPurchaseQueries request, CancellationToken ct)
        {
            var queryData = await _repository.GetAdminFiltered(request.PageIndex, request.Pagesize, request.Keyword);
            if (queryData != null && queryData.Count > 0)
            {
                return queryData;
            }
            return new List<ProductAdminFiltered>();
        }
    }
}
