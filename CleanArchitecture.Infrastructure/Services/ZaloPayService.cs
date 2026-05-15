using CleanArchitecture.Application.Services_Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Application.Model.PaymentModel;

namespace CleanArchitecture.Infrastructure.Services
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly IOptions<ZaloPayModel> _options;
        private readonly HttpClient _http;

        public ZaloPayService(IOptions<ZaloPayModel> options, HttpClient http)
        {
            _options = options;
            _http = http;
        }

        public async Task<string> CreatePaymentUrlAsync(Guid billId, long amount, string userName)
        {
            var opt = _options.Value;
            var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            // apptransid: yyMMdd_billId (tối đa 40 ký tự, unique mỗi đơn)
            var appTransId = $"{DateTime.Now:yyMMdd}_{billId.ToString().Replace("-", "")[..12]}";
            var embedData = JsonConvert.SerializeObject(new
            {
                redirecturl = opt.RedirectUrl + $"?billId={billId}"
            });
            var item = JsonConvert.SerializeObject(new[] {
            new { name = $"Đơn hàng #{billId}", quantity = 1, price = amount }
        });

            // Raw string ký
            var rawData = $"{opt.AppId}|{appTransId}|{userName}|{amount}|{appTime}|{embedData}|{item}";
            var mac = ComputeHmacSha256(opt.Key1, rawData);

            var payload = new Dictionary<string, string>
            {
                ["app_id"] = opt.AppId,
                ["app_user"] = userName,
                ["app_time"] = appTime.ToString(),
                ["amount"] = amount.ToString(),
                ["app_trans_id"] = appTransId,
                ["embed_data"] = embedData,
                ["item"] = item,
                ["callback_url"] = opt.CallbackUrl,
                ["description"] = $"Thanh toán đơn hàng #{billId}",
                ["bank_code"] = "",   // để trống → hiện tất cả phương thức
                ["mac"] = mac
            };

            var content = new FormUrlEncodedContent(payload);  // ZaloPay nhận form, không phải JSON
            var response = await _http.PostAsync(opt.CreateOrderUrl, content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);

            // return_code = 1 → thành công
            if ((int)result!.return_code != 1)
                throw new Exception($"ZaloPay lỗi: {result.return_message}");

            return (string)result.order_url;
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
