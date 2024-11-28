
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanGiaoDich_BrotherHood.Shared.Dto;
using System;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private IBill bill;
        public BillController(IBill bill)
        {
            this.bill = bill;
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
    }
}
