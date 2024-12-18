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
<<<<<<< HEAD
        Task<IEnumerable<Withdrawal_Infomation>> GetAllWithdrawals();
        Task<Withdrawal_Infomation> AddWithdrawal(Withdrawal_InfomationDto withdrawal);
        Task<Withdrawal_Infomation> UpdateWithDaral(int id, string status);
=======

        Task <IEnumerable<Withdrawal_information>> GetAllWithdrawals();
        Task<Withdrawal_information> AddWithdrawal(Withdrawal_informationDto withdrawal);
        Task<Withdrawal_information> UpdateWithDaral(int id, string status);
>>>>>>> 24a7e4152c1260d2c53227d58fdff1a674c01922
    }
}
