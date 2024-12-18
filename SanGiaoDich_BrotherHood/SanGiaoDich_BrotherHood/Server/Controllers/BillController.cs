
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanGiaoDich_BrotherHood.Shared.Dto;
using System;
using SanGiaoDich_BrotherHood.Server.Data;
using System.Linq;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private IBill bill;
        private readonly ApplicationDbContext _context;
        public BillController(IBill bill, ApplicationDbContext context)
        {
            this.bill = bill;
            _context = context;
        }

        [HttpGet("GetBill")]
        public async Task<IActionResult> GetBills()
        {
            return Ok(await bill.GetBills());
        }

        [HttpGet("GetBillsByUserName/{userName}")]
        public async Task<IActionResult> GetBillsByUserName(string userName)
        {
            return Ok(await bill.GetBillsByUserName(userName));
        }

        [HttpGet("GetBillsByIDBill/{IDBill}")]
        public async Task<IActionResult> GetBillsByIDBill(int IDBill)
        {
            return Ok(await bill.GetBillByIDBill(IDBill));
        }

        [HttpPost("AddBill")]
        public async Task<IActionResult> AddBill([FromBody] BillDto billDto)
        {
            if (billDto == null)
            {
                return BadRequest("Thông tin hóa đơn không hợp lệ.");
            }

            try
            {
                var newBill = await bill.AddBill(billDto);

                // Trả về phản hồi thành công với thông tin hóa đơn mới được tạo
                return Ok(newBill);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần thiết) và trả về phản hồi lỗi
                return StatusCode(500, $"Đã xảy ra lỗi khi tạo hóa đơn: {ex.Message}");
            }
        }


        [HttpPut("IDBill")]
        public async Task<ActionResult> UpdateBill(int IDBill, Bill bl)
        {
            return Ok(await bill.UpdateBill(IDBill, bl));
        }

        [HttpPost("AcceptBill/{idBill}")]
        public async Task<IActionResult> AcceptBill(int idBill)
        {
            return Ok(await bill.AcceptBill(idBill));
        }

        [HttpPost("CancelBill/{idBill}")]
        public async Task<IActionResult> CancelBill(int idBill)
        {
            return Ok(await bill.CancelBill(idBill));
        }

        [HttpPost("done/{id}")]
        public async Task<IActionResult> DoneBill(int id, string status)
        {
            var bi = await bill.DoneBill(id, status);

            if (bi == null)
            {
                return NotFound($"Bill with ID {id} not found.");
            }

            return Ok(bill);
        }

		[HttpGet("statistics")]
		public async Task<IActionResult> GetOrderStatistics()
		{
			var statistics = await bill.GetOrderStatisticsAsync();
			return Ok(statistics);
		}

        [HttpGet("GetBillsByDate")]
        public IActionResult GetBillsByDate(DateTime startDate, DateTime endDate)
        {
            var filteredBills = _context.Bills
                .Where(b => b.OrderDate.Date >= startDate.Date && b.OrderDate.Date <= endDate.Date)
                .ToList();
            return Ok(filteredBills);
        }
    }
}
