using AutoMapper;
using CleanArchitecture.Application.Features.Purchase;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.PurchaseHandlers
{
    public class AddNewPurchaseHandlers : IRequestHandler<AddNewPurchaseCommand, bool>
    {
        private readonly IPurchaseRepository _repository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ISupplierRepository _supplierRepository;

        public AddNewPurchaseHandlers(IPurchaseRepository repository, IPurchaseOrderRepository purchaseOrderRepository,  IMapper mapper, IProductRepository productRepository, ISupplierRepository supplierRepository)
        {
            _repository = repository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<bool> Handle(AddNewPurchaseCommand request, CancellationToken ct)
        {
            try
            {
                var MapperData = _mapper.Map<Purchase>(request);
                var purchaseData = new Purchase(MapperData.UserCreated, MapperData.SupplierId, MapperData.ImportPrice, MapperData.Description, DateTime.UtcNow, MapperData.State);
                //B1 : Tạo hóa đơn ( chưa có thông tin chi tiết )
                var createPurchase = await _repository.Add(purchaseData);
                //B2: Tạo hóa đơn nhập số liệu kho và cập nhật số lượng sản phẩm
                foreach (var data in request.PurchaseItems)
                {
                    var PurchaseOrderData = new PurchaseOrder(purchaseData.Id, data.ProductId, data.ImportPrice, data.Quantity, data.TotalPrice);
                    await _purchaseOrderRepository.Add(PurchaseOrderData);
                    var productUpdate = await _productRepository.GetProductDetail(data.ProductId);
                    /// Tính toán lại giá nhập và số lượng tồn kho
                    productUpdate.CalculateNewImportPrice(data.Quantity, data.ImportPrice);
                    await _productRepository.UpdateAsync(productUpdate);
                }
                await _supplierRepository.ClearCached(request.SupplierId);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception( "error:",ex);
            }
        }
    }
}
