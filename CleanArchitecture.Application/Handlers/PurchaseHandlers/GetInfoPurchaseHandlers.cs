using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Purchase;
using CleanArchitecture.Application.Features.Supplier;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;

namespace CleanArchitecture.Application.Handlers.PurchaseHandlers
{
    public class GetInfoPurchaseHandlers : IRequestHandler<GetInfoPurchaseQueries, PurchaseDetailModel>
    {
        private readonly IPurchaseRepository _repository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IMapper _mapper;

        public GetInfoPurchaseHandlers(IPurchaseRepository repository, IPurchaseOrderRepository purchaseOrderRepository, IMapper mapper)
        {
            _repository = repository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _mapper = mapper;
        }

        public async Task<PurchaseDetailModel> Handle(GetInfoPurchaseQueries request, CancellationToken ct)
        {
            var data = await _repository.GetInfoModel(request.Id);

            if (data == null)
                throw new KeyNotFoundException($"Không tìm thấy đơn nhập hàng với Id: {request.Id}");

            var items = await _purchaseOrderRepository.getPurchaseOrder(request.Id);
            data.PurchaseItems = items.ToList();
            var cachedkey = $"purchase:detail:{request.Id}";
            _repository.CachedPurchaseDetailData(cachedkey, data);
            return data;
        }
    }
}
