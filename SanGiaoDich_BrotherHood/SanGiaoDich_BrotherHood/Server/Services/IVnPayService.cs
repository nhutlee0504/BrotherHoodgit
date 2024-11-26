using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Shared.Models;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentRequestModel model, HttpContext context);

        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
