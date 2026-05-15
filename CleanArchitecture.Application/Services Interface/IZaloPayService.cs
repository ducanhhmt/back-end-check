using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Services_Interface
{
    public interface IZaloPayService
    {
        Task<string> CreatePaymentUrlAsync(Guid billId, long amount, string userName);
    }
}
