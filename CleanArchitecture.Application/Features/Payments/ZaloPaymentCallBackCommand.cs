using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Payments
{
    public class ZaloPaymentCallBackCommand : IRequest<bool>
    {
        public string Data { get; set; } = null!;
        public string Mac { get; set; } = null!;
    }
}
