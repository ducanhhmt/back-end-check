using CleanArchitecture.Application.Features.Payments;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Application.Services_Interface;
using CleanArchitecture.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CleanArchitectureLearning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ZaloPayController : ControllerBase
    {
        //private readonly IUserBillRepository _billRepo;
        private readonly ISender _sender;
        private readonly IConfiguration _config;
        public ZaloPayController(/*IUserBillRepository billRepo*/ ISender sender, IConfiguration config)
        {
            //_billRepo = billRepo;
            _sender = sender;
            _config = config;
        }
        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] ZaloPayCallbackRequest req)
        {
            //try
            //{
            //    Request.EnableBuffering();
            //    using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            //    var rawBody = await reader.ReadToEndAsync();

            //    var dataMatch = System.Text.RegularExpressions.Regex.Match(
            //        rawBody, @"""data""\s*:\s*""((?:[^""\\]|\\.)*)""");
            //    var macMatch = System.Text.RegularExpressions.Regex.Match(
            //        rawBody, @"""mac""\s*:\s*""([^""]+)""");

            //    var macReceived = macMatch.Groups[1].Value;
            //    var key2 = _config["ZaloPay:Key2"]!;

            //    var rawExtracted = dataMatch.Groups[1].Value;
            //    var unescaped1 = rawExtracted.Replace("\\\"", "\"").Replace("\\\\", "\\");

            //    var json = JObject.Parse(rawBody);
            //    var fromJObject = json["data"]!.Value<string>()!;
            //    var a = ComputeHmacSha256(key2, rawExtracted);
            //    var b = ComputeHmacSha256(key2, unescaped1);
            //    var c = ComputeHmacSha256(key2, fromJObject);
            //    Console.WriteLine($"MAC received : {macReceived}");
            //    Console.WriteLine($"MAC v1 (raw regex)  : {a}");
            //    Console.WriteLine($"MAC v2 (unescaped)  : {b}");
            //    Console.WriteLine($"MAC v3 (JObject)    : {c}");

            //    return Ok(new { return_code = 1, return_message = "success" });
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("error", ex.Message );
            //    return BadRequest();
            //}
            await _sender.Send(new ZaloPaymentCallBackCommand
            {
                Data = req.Data,
                Mac = req.Mac
            });

            // ZaloPay yêu cầu luôn trả về format này, dù thành công hay thất bại
            return Ok(new { return_code = 1, return_message = "success" });
        }
        private static string ComputeHmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(data)))
                .Replace("-", "").ToLower();
        }
    }

    public class ZaloPayCallbackRequest
    {
        public string Data { get; set; } = null!;
        public string Mac { get; set; } = null!;
        public int Type { get; set; }
    }
}
