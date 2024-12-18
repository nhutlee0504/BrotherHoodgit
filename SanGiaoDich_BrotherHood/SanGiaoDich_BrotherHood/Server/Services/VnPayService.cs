using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Server.Library;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public VnPayService(IConfiguration configuration, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Withdrawal_Infomation> AddWithdrawal(Withdrawal_InfomationDto withdrawal)
        {
            var user = GetUserInfoFromClaims();
            var newWI = new Withdrawal_Infomation
            {
                PaymentType = withdrawal.PaymentType,
                Amount = withdrawal.Amount,
                AccountNumber = withdrawal.AccountNumber,
                OrderDescription = user.FullName + "yêu cầu rút tiền",
                CreatedDate = DateTime.Now,
                Bank = withdrawal.Bank,
                Status = withdrawal.Status,
                UserName = user.UserName,
                FullName = user.FullName,
            };
            await _context.withdrawal_Infomations.AddAsync(newWI);
            await _context.SaveChangesAsync();
            return newWI;
        }

        public string CreatePaymentUrl(PaymentRequestModel model, HttpContext context)
        {

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = model.CreatedDate.Ticks.ToString();
            var pay = new VnPayLibrary();

            var urlCallBack = _configuration["Vnpay:PaymentBackReturnUrl"];
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.OrderDescription}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_TxnRef", $"{tick}");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            model.TxnRef = tick;
            _context.PaymentRequests.Add(model);
            _context.SaveChanges();
            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;

        }

        public async Task<IEnumerable<Withdrawal_Infomation>> GetAllWithdrawals()
        {
            return await _context.withdrawal_Infomations.ToListAsync();
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            var paymentResponse = _context.PaymentResponses
                .FirstOrDefault(pr => pr.OrderId == response.OrderId);
            if (paymentResponse != null && paymentResponse.IsProcessed)
            {
                return paymentResponse;
            }

            var paymentRequest = _context.PaymentRequests
                .FirstOrDefault(pr => pr.TxnRef == response.OrderId);

            if (paymentRequest == null)
            {
                throw new Exception("PaymentRequest not found");
            }

            var success = response.VnPayResponseCode == "00";

            paymentResponse = new PaymentResponseModel
            {
                Success = success,
                VnPayResponseCode = response.VnPayResponseCode,
                TransactionId = response.TransactionId,
                OrderId = response.OrderId,
                Token = response.Token,
                UserName = paymentRequest.UserName,
                Amount = (decimal)paymentRequest.Amount,
                PaymentMethod = "VnPay",
                PaymentId = response.PaymentId,
                OrderDescription = paymentRequest.OrderDescription,
                PaymentReqID = paymentRequest.PaymentReqID,
                IsProcessed = false
            };

            _context.PaymentResponses.Add(paymentResponse);
            _context.SaveChanges();
            if (success && !paymentResponse.IsProcessed)
            {
                var userName = paymentRequest.UserName;
                var amount = paymentRequest.Amount;

                AddFundsToAccount(userName, (decimal)amount);
                paymentResponse.IsProcessed = true;
                _context.SaveChanges();
            }

            return paymentResponse;
        }



        private void AddFundsToAccount(string userName, decimal amount)
        {
            try
            {
                var userAccount = _context.Accounts.FirstOrDefault(u => u.UserName == userName);
                if (userAccount != null)
                {
                    userAccount.PreSystem += amount;
                    _context.Accounts.Update(userAccount);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Người dùng không tồn tại.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cộng tiền vào tài khoản: {ex.Message}");
            }
        }

        private long TryParseLong(string value, string fieldName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new FormatException($"{fieldName} is missing or empty.");
            }

            if (!long.TryParse(value, out var result))
            {
                throw new FormatException($"{fieldName} is not a valid long integer. Value: '{value}'");
            }

            return result;
        }

        private (string UserName, string Email, string FullName, string PhoneNumber, string Gender, string IDCard, DateTime? Birthday, string ImageAccount, string Role, bool IsDelete, DateTime? TimeBanned) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                // Kiểm tra thời gian hết hạn của token
                var expirationClaim = userClaim.FindFirst("exp");
                if (expirationClaim != null && long.TryParse(expirationClaim.Value, out long exp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    if (expirationTime < DateTime.UtcNow)
                    {
                        throw new UnauthorizedAccessException("Token đã hết hạn. Vui lòng đăng nhập lại.");
                    }
                }

                var userNameClaim = userClaim.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                var fullNameClaim = userClaim.FindFirst("FullName")?.Value ?? string.Empty;
                var phoneNumberClaim = userClaim.FindFirst("PhoneNumber")?.Value ?? string.Empty;
                var genderClaim = userClaim.FindFirst("Gender")?.Value ?? string.Empty;
                var idCardClaim = userClaim.FindFirst("IDCard")?.Value ?? string.Empty;
                var imageAccountClaim = userClaim.FindFirst("ImageAccount")?.Value ?? string.Empty;
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

                DateTime? birthday = null;
                var birthdayClaimValue = userClaim.FindFirst("Birthday")?.Value;
                if (!string.IsNullOrWhiteSpace(birthdayClaimValue) && DateTime.TryParse(birthdayClaimValue, out DateTime parsedBirthday))
                {
                    birthday = parsedBirthday;
                }

                bool isDelete = false;
                var isDeleteClaimValue = userClaim.FindFirst("IsDelete")?.Value;
                if (isDeleteClaimValue != null && bool.TryParse(isDeleteClaimValue, out bool parsedIsDeleted))
                {
                    isDelete = parsedIsDeleted;
                }

                DateTime? timeBanned = null;
                var timeBannedClaimValue = userClaim.FindFirst("TimeBanned")?.Value;
                if (!string.IsNullOrWhiteSpace(timeBannedClaimValue) && DateTime.TryParse(timeBannedClaimValue, out DateTime parsedTimeBanned))
                {
                    timeBanned = parsedTimeBanned;
                }

                return (
                    userNameClaim,
                    emailClaim,
                    fullNameClaim,
                    phoneNumberClaim,
                    genderClaim,
                    idCardClaim,
                    birthday,
                    imageAccountClaim,
                    roleClaim,
                    isDelete,
                    timeBanned
                );
            }

            throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.");
        }

        public async Task<Withdrawal_Infomation> UpdateWithDaral(int id, string status)
        {
            var FInd = await _context.withdrawal_Infomations.FindAsync(id);
            if (FInd == null)
            {
                throw new NotImplementedException();
            }
            FInd.Status = status;
            await _context.SaveChangesAsync();
            return FInd;
        }
    }
}
