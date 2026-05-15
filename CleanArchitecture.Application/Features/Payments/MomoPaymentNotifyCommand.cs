using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Payments
{
    public class MomoPaymentNotifyCommand : IRequest<bool>
    {
        public int ResultCode { get; set; }
        public string OrderInfo { get; set; } = null!;
        public string OrderId { get; set; } = null!;
        public long Amount { get; set; }
        public long TransId { get; set; }
    }
}
