using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Model.PaymentModel
{
    public class ZaloPayModel
    {
        public string AppId { get; set; } = null!;
        public string Key1 { get; set; } = null!;
        public string Key2 { get; set; } = null!;
        public string CreateOrderUrl { get; set; } = null!;
        public string CallbackUrl { get; set; } = null!;
        public string RedirectUrl { get; set; } = null!;
    }
}
