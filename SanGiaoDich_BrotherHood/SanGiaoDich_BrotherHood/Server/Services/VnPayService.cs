using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Server.Library;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Linq;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration , ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
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
            pay.AddRequestData("vnp_IpAddr",pay.GetIpAddress(context));
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


    }
}
