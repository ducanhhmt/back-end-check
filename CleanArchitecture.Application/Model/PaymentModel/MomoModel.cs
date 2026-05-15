using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model.PaymentModel
{
    /// <summary>
    /// Map với appsettings.json → section "MomoAPI" (từ doc)
    /// </summary>
    public class MomoOptionModel
    {
        public string MomoApiUrl { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
        public string NotifyUrl { get; set; } = null!;
        public string PartnerCode { get; set; } = null!;
        public string RequestType { get; set; } = null!;
    }

    /// <summary>
    /// Response từ MoMo trả về sau khi tạo lệnh thanh toán (từ doc)
    /// </summary>
    public class MomoCreatePaymentResponseModel
    {
        public string RequestId { get; set; } = null!;
        public int ErrorCode { get; set; }
        public string OrderId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string LocalMessage { get; set; } = null!;
        public string RequestType { get; set; } = null!;
        public string PayUrl { get; set; } = null!;
        public string Signature { get; set; } = null!;
        public string QrCodeUrl { get; set; } = null!;
        public string Deeplink { get; set; } = null!;
        public string DeeplinkWebInApp { get; set; } = null!;
    }

    /// <summary>
    /// Dữ liệu MoMo trả về qua ReturnUrl / NotifyUrl (từ doc)
    /// </summary>
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string OrderInfo { get; set; } = null!;
    }

    /// <summary>
    /// Input để tạo lệnh thanh toán — thêm BillId để map với bill trong DB
    /// </summary>
    public class OrderInfoModel
    {
        public Guid BillId { get; set; }   // ← link về UserBill
        public string FullName { get; set; } = null!;
        public long Amount { get; set; }
        public string OrderInfo { get; set; } = null!;
        public string OrderId { get; set; } = null!; // MoMo điền vào (Ticks)
    }
}
