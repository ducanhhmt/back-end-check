using CleanArchitecture.Application.Model.PaymentModel;
using CleanArchitecture.Application.Services_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitecture.Infrastructure.Services
{
    // --- MomoService.cs ---
    // Giữ đúng logic từ doc: raw string, HMAC-SHA256, deserialize response
    // Thay RestSharp bằng HttpClient (không cần cài thêm package)
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly HttpClient _httpClient;
        public MomoService(IOptions<MomoOptionModel> options, HttpClient httpClient)
        {
            _options = options;
            _httpClient = httpClient;
        }

        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model)
        {
            // Đặt OrderId = Ticks (giữ đúng như doc)
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInfo = $"Khách hàng: {model.FullName}. Nội dung: {model.OrderInfo}";
            // Raw string ký — thứ tự field PHẢI khớp đúng như doc
            //var rawData =
            //    $"partnerCode={_options.Value.PartnerCode}" +
            //    $"&accessKey={_options.Value.AccessKey}" +
            //    $"&requestId={model.OrderId}" +
            //    $"&amount={model.Amount}" +
            //    $"&orderId={model.OrderId}" +
            //    $"&orderInfo={model.OrderInfo}" +
            //    $"&returnUrl={_options.Value.ReturnUrl}" +
            //    $"&notifyUrl={_options.Value.NotifyUrl}" +
            //    $"&extraData=";

            var rawData =
                $"accessKey={_options.Value.AccessKey}" +
                $"&amount={model.Amount}" +
                $"&extraData=" +
                $"&ipnUrl={_options.Value.NotifyUrl}" +  // NotifyUrl → ipnUrl
                $"&orderId={model.OrderId}" +
                $"&orderInfo={model.OrderInfo}" +
                $"&partnerCode={_options.Value.PartnerCode}" +
                $"&redirectUrl={_options.Value.ReturnUrl}" +  // returnUrl → redirectUrl
                $"&requestId={model.OrderId}" +
                $"&requestType={_options.Value.RequestType}";
            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            // Payload gửi lên MoMo (từ doc)
            //var requestData = new
            //{
            //    accessKey = _options.Value.AccessKey,
            //    partnerCode = _options.Value.PartnerCode,
            //    requestType = _options.Value.RequestType,
            //    notifyUrl = _options.Value.NotifyUrl,
            //    returnUrl = _options.Value.ReturnUrl,
            //    orderId = model.OrderId,
            //    amount = model.Amount.ToString(),   // MoMo v1 nhận string
            //    orderInfo = model.OrderInfo,
            //    requestId = model.OrderId,
            //    extraData = "",
            //    signature = signature
            //};
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                ipnUrl = _options.Value.NotifyUrl,      // ← đổi
                redirectUrl = _options.Value.ReturnUrl,       // ← đổi
                orderId = model.OrderId,
                amount = model.Amount,                   // v2 nhận long, không phải string
                orderInfo = model.OrderInfo,
                requestId = model.OrderId,
                extraData = "",
                signature = signature,
                lang = "vi"
            };
            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_options.Value.MomoApiUrl, content);
            var responseStr = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(responseStr)
                   ?? throw new Exception("MoMo không trả về dữ liệu hợp lệ");
        }

        /// <summary>
        /// Parse query string MoMo redirect về ReturnUrl (từ doc)
        /// </summary>
        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection.First(s => s.Key == "amount").Value;
            var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
            var orderId = collection.First(s => s.Key == "orderId").Value;

            return new MomoExecuteResponseModel
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                FullName = "HaiMT"
            };
        }

        // Giữ nguyên hàm HMAC từ doc
        private static string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

}
