using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using System;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class WithdrawalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WithdrawalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yêu cầu rút tiền
        [HttpPost("request")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] WithdrawalRequest model)
        {
            if (model == null || model.Amount <= 0)
            {
                return BadRequest("Số tiền rút không hợp lệ.");
            }

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            var withdrawalRequest = new WithdrawalRequest
            {
                UserName = model.UserName,
                Amount = model.Amount,
                RequestDate = model.RequestDate,
                Status = "Pending"
            };

            _context.WithdrawalRequests.Add(withdrawalRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Yêu cầu rút tiền đã được gửi thành công." });
        }

        // Xử lý yêu cầu rút tiền (admin sẽ phê duyệt hoặc từ chối)
        [HttpPut("process/{id}")]
        public async Task<IActionResult> ProcessWithdrawalRequest(int id, [FromBody] string status)
        {
            var withdrawalRequest = await _context.WithdrawalRequests.FindAsync(id);
            if (withdrawalRequest == null)
            {
                return NotFound("Yêu cầu rút tiền không tồn tại.");
            }

            if (status != "Approved" && status != "Rejected")
            {
                return BadRequest("Trạng thái không hợp lệ.");
            }

            withdrawalRequest.Status = status;
            _context.WithdrawalRequests.Update(withdrawalRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Yêu cầu rút tiền đã được xử lý." });
        }

        // Lấy danh sách yêu cầu rút tiền
        [HttpGet("requests")]
        public async Task<IActionResult> GetWithdrawalRequests()
        {
            var requests = await _context.WithdrawalRequests.ToListAsync();
            return Ok(requests);
        }
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserWithdrawalRequests(string username)
        {
            var requests = await _context.WithdrawalRequests
                .Where(r => r.UserName == username)
                .ToListAsync();

            if (requests == null || !requests.Any())
            {
                return NotFound("Không có yêu cầu rút tiền nào.");
            }

            return Ok(requests);
        }
        public class WithdrawalRequest
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public decimal Amount { get; set; }
            public DateTime RequestDate { get; set; }
            public string Status { get; set; }
        }

    }
   
}
