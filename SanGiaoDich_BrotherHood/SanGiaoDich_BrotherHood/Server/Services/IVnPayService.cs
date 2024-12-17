using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentRequestModel model, HttpContext context);

        PaymentResponseModel PaymentExecute(IQueryCollection collections);

        Task <IEnumerable<Withdrawal_information>> GetAllWithdrawals();
        Task<Withdrawal_information> AddWithdrawal(Withdrawal_informationDto withdrawal);
        Task<Withdrawal_information> UpdateWithDaral(int id, string status);
    }
}
