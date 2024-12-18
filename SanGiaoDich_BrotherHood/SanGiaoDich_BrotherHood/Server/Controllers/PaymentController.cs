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
using Net.payOS.Types;
using Net.payOS;
using SanGiaoDich_BrotherHood.Shared.Dto;

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
		[HttpPost("create-receive-qr")]
		public async Task<IActionResult> CreateReceiveQr()
		{
			var clientId = "72f89b23-fde5-4396-8c23-328ddbe5e90a";
			var apiKey = "1bc41f02-384e-4689-a41b-49c76e757a07";
			var checksumKey = "fe4fd7424bc7faf90386006c26a9ff43ff9d1afe01d8da214544e1416c4cd61e";

			var payOS = new PayOS(clientId, apiKey, checksumKey);

			var domain = "https://localhost:5001"; // Đường dẫn ứng dụng của bạn
			var items = new List<Net.payOS.Types.ItemData>
	{
		new Net.payOS.Types.ItemData("Nhận tiền từ ví", 1, 0) // Không cần sản phẩm cụ thể
    };

			var paymentLinkRequest = new PaymentData(
				orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
				amount: 50000, // Số tiền muốn nhận
				description: "Nhận tiền thông qua QR",
				items: items,
				returnUrl: domain + "/success",
				cancelUrl: domain + "/cancel"
			);

			try
			{
				var response = await payOS.createPaymentLink(paymentLinkRequest);
				return Ok(new { QrUrl = response.checkoutUrl });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = $"Lỗi: {ex.Message}", details = ex.StackTrace });
			}
		}

		[HttpGet("withdrawals")]
		public async Task<IActionResult> GetAllWithdrawals()
		{
			var withdrawals = await _vnPayService.GetAllWithdrawals();
			return Ok(withdrawals);
		}

		[HttpPost("withdrawals")]
		public async Task<IActionResult> AddWithdrawal(Withdrawal_InfomationDto withdrawal)
		{
			if (withdrawal == null)
			{
				return BadRequest("Thông tin yêu cầu rút tiền không hợp lệ.");
			}

			var newWithdrawal = await _vnPayService.AddWithdrawal(withdrawal);
			return Ok(newWithdrawal);
		}
		[HttpPut("update-withdrawal/{id}/{status}")]
		public async Task<IActionResult> UpdateWithdrawal(int id, string status)
		{
			try
			{
				// Gọi phương thức UpdateWithDaral từ service
				var updatedWithdrawal = await _vnPayService.UpdateWithDaral(id, status);

				if (updatedWithdrawal == null)
				{
					return NotFound("Đơn rút tiền không tồn tại.");
				}

				return Ok(new { message = "Cập nhật yêu cầu rút tiền thành công", withdrawal = updatedWithdrawal });
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi server: {ex.Message}");
			}
		}
	}
}
