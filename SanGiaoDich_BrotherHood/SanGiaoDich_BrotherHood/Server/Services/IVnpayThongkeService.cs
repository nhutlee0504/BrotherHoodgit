using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SanGiaoDich_BrotherHood.Server.Services.VnpayThongkeService;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IVnpayThongkeService
    {
        Task<List<PaymentResponseModel>> GetAllPaymentResponsesAsync();
        Task<List<PaymentRequestModel>> GetAllPaymentRequestsAsync();
        Task<List<PaymentRequestModel>> GetAllPaymentRequestsWithUser(string username);
    }
}
