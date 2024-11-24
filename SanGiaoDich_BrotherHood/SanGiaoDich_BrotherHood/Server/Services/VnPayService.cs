using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SanGiaoDich_BrotherHood.Server.Library;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Linq;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
      
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = model.CreatedDate.Ticks.ToString();
            var pay = new Library.VnPayLibrary();
            var urlCallBack = _configuration["VnPay:PaymentBackReturnUrl"];
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.PaymentID},{model.OrderDescription},{model.Amount}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_TxnRef", tick);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
         

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;

        }


        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
                var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;

            
        }

        // Phương thức hỗ trợ chuyển đổi an toàn từ chuỗi sang số nguyên dài (long)
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
