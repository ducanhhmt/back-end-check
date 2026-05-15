using CleanArchitecture.Application.Features.Payments;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MomoController : ControllerBase
    {
        //private readonly IMomoService _momoService;
        //private readonly IUserBillRepository _billRepo;
        private readonly ISender _sender;
        public MomoController(/*IMomoService momoService, IUserBillRepository billRepo*/ ISender sender)
        {
            //_momoService = momoService;
            //_billRepo = billRepo;
            _sender = sender;
        }
        //public async Task<IActionResult> Notify([FromQuery] IQueryCollection collection)
        //{
        //    var result = _momoService.PaymentExecuteAsync(Request.Query);
        //    // errorCode=0 → thanh toán thành công
        //    var errorCode = Request.Query.FirstOrDefault(s => s.Key == "errorCode").Value.ToString();
        //    if (errorCode == "0" && Guid.TryParse(result.OrderId, out var billId))
        //    {
        //        // Lưu ý: OrderId của MoMo là Ticks, không phải Guid
        //        // → Cần thêm mapping BillId↔MoMoOrderId khi tạo bill
        //        // (xem ghi chú phía dưới)
        //    }
        //    return Ok(); // MoMo yêu cầu trả 200 để biết đã nhận
        //}
        /// <summary>
        /// MoMo server gọi vào đây sau khi thanh toán (NotifyUrl — server-to-server)
        /// Đây là nơi tin tưởng để cập nhật trạng thái bill
        /// </summary>
        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromBody] MomoIpnRequest request)
        {
            var result = await _sender.Send(new MomoPaymentNotifyCommand
            {
                ResultCode = request.ResultCode,
                OrderInfo = request.OrderInfo,
                OrderId = request.OrderId,
                Amount = request.Amount,
                TransId = request.TransId
            });
            return Ok();
        }

        /// <summary>
        /// Browser redirect về đây sau khi người dùng thanh toán xong (ReturnUrl) hiện chưa cần vì returnUrl về bên ui angular
        /// Chỉ dùng để hiển thị UI, KHÔNG dùng để cập nhật trạng thái
        /// </summary>
        //[HttpGet("return")]
        //public IActionResult Return()
        //{
        //    var result = _momoService.PaymentExecuteAsync(Request.Query);
        //    var errorCode = Request.Query.FirstOrDefault(s => s.Key == "errorCode").Value.ToString();

        //    if (errorCode == "0")
        //        return Ok(new { success = true, orderId = result.OrderId, amount = result.Amount });

        //    return BadRequest(new { success = false, message = "Thanh toán thất bại hoặc bị hủy" });
        //}

        public class MomoIpnRequest
        {
            public string PartnerCode { get; set; }
            public string OrderId { get; set; }
            public string RequestId { get; set; }
            public long Amount { get; set; }
            public string OrderInfo { get; set; }
            public string OrderType { get; set; }
            public long TransId { get; set; }
            public int ResultCode { get; set; }  // 0 = thành công
            public string Message { get; set; }
            public string PayType { get; set; }
            public long ResponseTime { get; set; }
            public string ExtraData { get; set; }
            public string Signature { get; set; }
        }
    }
}
