using AutoMapper;
using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.Bill;
using CleanArchitecture.Application.Features.Carts;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Products;
using CleanArchitecture.Application.Model;
using CleanArchitecture.Application.Model.PaymentModel;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Handlers.BillHandlers
{
    public class CreateBillHandlers : IRequestHandler<CreateBillCommand, UserBillDTO>
    {
        private readonly IProductRepository _productRepository;
        private readonly IBillInfoRepository _billInfoRepository;
        private readonly IUserBillRepository _userBillRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IMomoService _momoService; // ← inject thêm
        private readonly IZaloPayService _zalopayService;

        public CreateBillHandlers(IProductRepository productRepository, IBillInfoRepository billInfoRepository,
            IUserBillRepository userBillRepository, ICartRepository cartRepository, IMapper mapper, IMomoService momoService, IZaloPayService zalopayService)
        {
            _productRepository = productRepository;
            _billInfoRepository = billInfoRepository;
            _userBillRepository = userBillRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _momoService = momoService;
            _zalopayService = zalopayService;
        }

        public async Task<UserBillDTO> Handle(CreateBillCommand request, CancellationToken ct)
        {
            // COD → status 1 (đã thanh toán luôn)
            // MoMo → status 0 (chờ thanh toán, cập nhật sau qua NotifyUrl)
            int initialStatus = request.PaymentMethod == "cod" ? 0 : 4;
            //B1 : Tạo hóa đơn ( chưa có thông tin chi tiết )
            var userBilldata = new UserBill(request.userId.Value,request.userName, request.Address, request.Phone, request.TotalPrice, initialStatus, request.ShippingPrice,request.DiscountPrice, DateTime.UtcNow);
            var createUserBill = await _userBillRepository.Add(userBilldata);
            //B2: Dùng for tạo thông tin chi tiết cho hóa đơn và Trừ số lượng sản phẩm trong kho theo hóa đơn
            var billinfodata = new List<BillInfo>();
            foreach (var data in request.Items)
            {
                var productInfo = await _productRepository.GetProductDetail(data.ProductId);
                var productInfoMapper = _mapper.Map<ProductAdminDetailDto>(productInfo);
                if (productInfo != null)
                {
                    // Tạo Thông tin chi tiết
                    var billInfo = new BillInfo(createUserBill.Id, data.ProductId, productInfoMapper.Name, data.Quantity,
                    productInfoMapper.Price, productInfoMapper.ImportPrice, productInfoMapper.Price * data.Quantity, productInfoMapper.Images[0]);
                    var addBillInfo = await _billInfoRepository.AddAsync(billInfo);
                    billinfodata.Add(addBillInfo);
                    //Trừ kho
                    productInfo.UpdateStock(productInfo.Quantity - data.Quantity);
                    var updateQuantityPD = await _productRepository.UpdateAsync(productInfo);
                    // xóa sản phẩm khỏi giỏ hàng
                    var removecart = await _cartRepository.RemoveCartOnUser(request.userId.Value, productInfo.Id);
                }              
            }
            var resultData = _mapper.Map<UserBillDTO>(userBilldata);
            resultData.Items = billinfodata;
            // B4: Nếu là MoMo → gọi MomoService tạo payUrl (từ doc)
            if (request.PaymentMethod == "momo")
            {
                var momoModel = new OrderInfoModel
                {
                    BillId = createUserBill.Id,
                    FullName = request.userName,
                    Amount = (long)request.TotalPrice,
                    OrderInfo = $"Thanh toán đơn hàng #{createUserBill.Id}"
                };
                var momoResponse = await _momoService.CreatePaymentAsync(momoModel);
                // ErrorCode = 0 → tạo thành công
                if (momoResponse.ErrorCode != 0)
                    throw new Exception($"MoMo lỗi: {momoResponse.Message}");
                resultData.PayUrl = momoResponse.PayUrl; // trả về Angular để redirect
            }
            if (request.PaymentMethod == "zalopay")
            {
                var payUrl = await _zalopayService.CreatePaymentUrlAsync(
                    createUserBill.Id,
                    (long)request.TotalPrice,
                    request.userName);
                resultData.PayUrl = payUrl;
            }
            return resultData;
        }
    }
}
