using CleanArchitecture.Application.Model.PaymentModel;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Application.Services_Interface
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
