using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Bill;
using CleanArchitecture.Application.Features.Carts;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Xml.XPath;

namespace CleanArchitecture.Application.Handlers.BillHandlers
{
    public class RemoveBillHandlers : IRequestHandler<RemoveBillCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBillInfoRepository _billInfoRepository;
        private readonly IUserBillRepository _userBillRepository;
        private readonly IMapper _mapper;

        public RemoveBillHandlers(IBillInfoRepository billInfoRepository, IUserBillRepository userBillRepository, IMapper mapper, IProductRepository productRepository)
        {
            _billInfoRepository = billInfoRepository;
            _userBillRepository = userBillRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(RemoveBillCommand request, CancellationToken ct)
        {
            // Lấy thông tin chi tiết trong hóa đơn ( thông tin sản phẩm và số lượng) để cập nhật lại số lượng kho hàng
            var lstPrductonBill = await _billInfoRepository.GetById(request.BillId);
            foreach( var pd in lstPrductonBill)
            {
                var productInfo = _mapper.Map<Product>( await _productRepository.GetAdminDetail(pd.ProductId));
                if (productInfo != null)
                {
                    productInfo.UpdateStock(productInfo.Quantity + pd.Quantity);
                    await _productRepository.UpdateAsync(productInfo);
                }
            }
            // Cập nhật trạng thái hủy đơn hàng ( State = 3 )
            var billData = await _userBillRepository.GetById(request.BillId);
            //xóa cached trước khi cập nhật state đơn hàng
            await _userBillRepository.InvalidateUserBillListCache(billData.UserId, 3);
            await _userBillRepository.InvalidateUserBillListCache(billData.UserId, billData.State);
            billData.UpdateState(3);
            await _userBillRepository.Update(billData);       
            return true;
        }
    }
}
