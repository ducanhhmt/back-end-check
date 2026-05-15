using CleanArchitecture.Application.Features.Payments;
using CleanArchitecture.Application.Repository_Interface;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace CleanArchitecture.Application.Handlers.Payments
{
    public class ZaloPaymentCallBackHandler : IRequestHandler<ZaloPaymentCallBackCommand, bool>
    {
        private readonly IUserBillRepository _billRepo;
        private readonly IConfiguration _config;

        public ZaloPaymentCallBackHandler(IUserBillRepository billRepo, IConfiguration config)
        {
            _billRepo = billRepo;
            _config = config;
        }
        public async Task<bool> Handle(ZaloPaymentCallBackCommand request, CancellationToken ct)
        {
            var key2 = _config["ZaloPay:Key2"];
            // Verify MAC dùng Key2 (khác với tạo order dùng Key1)
            var expectedMac = ComputeHmacSha256(key2!, request.Data.Normalize());
            if (expectedMac != request.Mac)
            {
                Console.WriteLine("[ZaloPayCallback] MAC không hợp lệ");
                return false;
            }
            // Parse data JSON
            var dataObj = JsonConvert.DeserializeObject<dynamic>(request.Data)!;
            var embedData = JsonConvert.DeserializeObject<dynamic>((string)dataObj.embed_data)!;
            // Lấy billId từ embedData.redirecturl
            var redirectUrl = (string)embedData.redirecturl;
            var billIdStr = redirectUrl.Split("billId=").LastOrDefault()?.Trim();
            if (string.IsNullOrEmpty(billIdStr) || !Guid.TryParse(billIdStr, out var billId))
            {
                Console.WriteLine($"[ZaloPayCallback] Không parse được BillId: {redirectUrl}");
                return false;
            }
            var bill = await _billRepo.GetById(billId);
            if (bill == null)
            {
                Console.WriteLine($"[ZaloPayCallback] Không tìm thấy bill: {billId}");
                return false;
            }
            // Cập nhật trạng thái bill → 0 = đang xử lý (đã thanh toán thành công)
            bill.UpdateState(0);
            await _billRepo.Update(bill);
            // Invalidate cache detail + list state = 4 (chờ thanh toán) page 1
            // (invalidate rộng hơn nếu cần — xóa tất cả page của state 4)
            await _billRepo.InvalidateUserBilllCache(billId);
            await _billRepo.InvalidateUserBillListCache(bill.UserId, 4);
            await _billRepo.InvalidateUserBillListCache(bill.UserId, 0);
            return true;
        }

        private static string ComputeHmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(data)))
                .Replace("-", "").ToLower();
        }
    }
}
