using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly HttpClient _httpClient;

        public PaymentController(IVnPayService vnPayService, HttpClient httpClient)
        {
            _vnPayService = vnPayService;
            _httpClient = httpClient;
        }

        [HttpPost("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
 
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { message = "Không thể tạo URL thanh toán" });
            }

            return Ok(new { paymentUrl = url });
        }
        //[HttpGet("PaymentCallbackVnpay")]
        //public IActionResult PaymentCallbackVnpay()
        //{
        //    Response.Headers["Cache-Control"] = "no-store";
        //    var response = _vnPayService.PaymentExecute(Request.Query);
        //    return Ok(response);
        //}
        [HttpGet("PaymentCallbackVnpay")]
        public IActionResult PaymentCallbackVnpay()
        {
            try
            {
                // Đảm bảo không lưu cache khi trả về dữ liệu từ API
                Response.Headers["Cache-Control"] = "no-store";

                // Lấy dữ liệu phản hồi từ VNPay
                var response = _vnPayService.PaymentExecute(Request.Query);

                // Kiểm tra nếu response hợp lệ
                if (response == null)
                {
                    // Trả về JSON nếu không nhận được phản hồi từ VNPay
                    return new JsonResult(new { Success = false, Message = "Không nhận được dữ liệu từ VNPay." })
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                // Trả về phản hồi từ VNPay dưới dạng JSON
                return new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ngoại lệ
                return new JsonResult(new { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }



        public bool VerifyVNPAYHash(Dictionary<string, string> queryParams, string secretKey, string secureHash)
        {
            // Sắp xếp các tham số theo thứ tự alphabet
            var sortedParams = queryParams
                .Where(p => p.Key != "vnp_SecureHash") // Loại bỏ vnp_SecureHash
                .OrderBy(p => p.Key)
                .Select(p => $"{p.Key}={p.Value}")
                .ToList();

            // Tạo chuỗi dữ liệu
            var data = string.Join("&", sortedParams);

            // Tính toán mã băm với HMAC SHA256
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(data));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // So sánh mã băm tính được với mã băm gửi về từ VNPay
                return hashString == secureHash.ToLower();
            }
        }

    }
}
