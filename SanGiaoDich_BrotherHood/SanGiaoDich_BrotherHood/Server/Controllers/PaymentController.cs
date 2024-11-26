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
using SanGiaoDich_BrotherHood.Server.Library;
using Microsoft.Extensions.Configuration;
using SanGiaoDich_BrotherHood.Server.Data;
using Microsoft.EntityFrameworkCore;
using static SanGiaoDich_BrotherHood.Client.Pages.Dashboard;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IVnpayThongkeService _vnpayThongkeService;
        private readonly ApplicationDbContext _context;
        public PaymentController(IVnPayService vnPayService, HttpClient httpClient, IConfiguration configuration, IVnpayThongkeService vnpayThongkeService, ApplicationDbContext context)
        {
            _vnPayService = vnPayService;
            _httpClient = httpClient;
            _configuration = configuration;
            _vnpayThongkeService = vnpayThongkeService;
            _context = context;
        }

        [HttpPost("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(PaymentRequestModel model)
        {
 
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new { message = "Không thể tạo URL thanh toán" });
            }

            return Ok(new { paymentUrl = url });
        }
        [HttpGet("PaymentCallbackVnpay")]
        public IActionResult PaymentCallbackVnpay()
        {
            if (!Request.Query.Any())
            {
                Console.WriteLine("Không có query parameters nào được nhận.");
            }
            else
            {
                foreach (var key in Request.Query.Keys)
                {
                    Console.WriteLine($"{key}: {Request.Query[key]}");
                }
            }

            // Thực hiện xử lý tiếp theo
            var response = _vnPayService.PaymentExecute(Request.Query);
            return new JsonResult(response);
        }
        [HttpGet("responses")]
        public async Task<IActionResult> GetPaymentResponses()
        {
            var responses = await _vnpayThongkeService.GetAllPaymentResponsesAsync();
            return Ok(responses);
        }
        [HttpGet("request-user/{username}")]
        public async Task<IActionResult> GetPaymentRequestsByUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Tên người dùng không hợp lệ.");
            }

            try
            {
                var paymentRequests = await _vnpayThongkeService.GetAllPaymentRequestsWithUser(username);


                if (paymentRequests == null || !paymentRequests.Any())
                {
                    return NotFound("Không tìm thấy yêu cầu thanh toán nào cho người dùng này.");
                }
                return Ok(paymentRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }



        [HttpGet("requests")]
        public async Task<IActionResult> GetPaymentRequests()
        {
            var requests = await _vnpayThongkeService.GetAllPaymentRequestsAsync();
            return Ok(requests);
        }
        [HttpGet("payment-data")]
        public IActionResult GetPaymentData()
        {
            var paymentData = _context.PaymentResponses
                .Include(pr => pr.PaymentRequest)
                .Select(pr => new PaymentDataModel
                {
                    OrderId = pr.OrderId,
                    UserName = pr.UserName,
                    Amount = pr.Amount,
                    OrderDescription = pr.OrderDescription,
                    Success = pr.Success,
                    CreatedDate = pr.PaymentRequest.CreatedDate,
                    PaymentMethod = pr.PaymentMethod
                }).ToList();

            return Ok(paymentData);
        }

        public class PaymentDataModel
        {
            public string OrderId { get; set; }
            public string UserName { get; set; }
            public decimal Amount { get; set; }
            public string OrderDescription { get; set; }
            public bool Success { get; set; }
            public DateTime CreatedDate { get; set; }
            public string PaymentMethod { get; set; }
        }

    }
}
