using AutoMapper;
using CleanArchitecture.Application.Features.Purchase;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.PurchaseHandlers
{
    public class UpdatePurchaseHandlers : IRequestHandler<UpdatePurchaseCommand, bool>
    {
        private readonly IPurchaseRepository _repository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IMapper _mapper;

        public UpdatePurchaseHandlers(IPurchaseRepository repository,  IMapper mapper, ISupplierRepository supplierRepository, IProductRepository productRepository, IPurchaseOrderRepository purchaseOrderRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<bool> Handle(UpdatePurchaseCommand request, CancellationToken ct)
        {
            try
            {
                var dataNeedupdate = await _repository.GetbyId(request.Id);
                if (dataNeedupdate != null) {
                    // ✅ Nếu hủy đơn → hoàn lại số liệu kho
                    if (request.State == 2 && dataNeedupdate.State != 2)
                    {
                        // Lấy danh sách sản phẩm trong đơn nhập
                        var purchaseItems = await _purchaseOrderRepository.getPurchaseOrder(dataNeedupdate.Id);

                        foreach (var item in purchaseItems)
                        {
                            var product = await _productRepository.GetProductDetail(item.ProductId);
                            // Hoàn lại số lượng và giá nhập về trước khi nhập hàng
                            product.RollbackImportPrice(item.Quantity, item.ImportPrice);
                            await _productRepository.UpdateAsync(product);
                        }
                    }
                    dataNeedupdate.UpdateInfo(request.Description, request.State);
                    await _repository.Update(dataNeedupdate);
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
