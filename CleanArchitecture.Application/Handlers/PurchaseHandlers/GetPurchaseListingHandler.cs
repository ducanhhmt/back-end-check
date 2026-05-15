using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Features.Purchase;
using CleanArchitecture.Application.Features.Supplier;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.PurchaseHandlers
{
    public class GetPurchaseListingHandler : IRequestHandler<PaginationPurchaseQueries, PurchasePaginationRespone>
    {
        private readonly IPurchaseRepository _repository;
        private readonly IMapper _mapper;

        public GetPurchaseListingHandler(IPurchaseRepository repository, IMapper mapper)
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
        public async Task<PurchasePaginationRespone> Handle(PaginationPurchaseQueries request, CancellationToken ct)
        {
            var queryData = await _repository.PaginationFilter(request.Keyword, request.PageIndex, request.Pagesize);
            if (queryData != null && queryData.Count > 0)
            {
                return new PurchasePaginationRespone
                {
                    data = queryData,
                    TotalRecords = queryData.FirstOrDefault()?.TotalRecords ?? 0,
                    pageCount = queryData.FirstOrDefault()?.TotalPages ?? 0,
                };
            }
            return new PurchasePaginationRespone();
        }
    }
}
