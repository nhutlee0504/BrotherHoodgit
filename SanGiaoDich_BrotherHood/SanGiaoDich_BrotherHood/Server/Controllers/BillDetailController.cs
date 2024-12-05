
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanGiaoDich_BrotherHood.Shared.Dto;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDetailController : ControllerBase
    {
        private IBillDetail billDetail;
        public BillDetailController(IBillDetail billDetail)
        {
            this.billDetail = billDetail;
        }

        [HttpGet]
        public async Task<ActionResult> GetBillDetails()
        {
            return Ok(await billDetail.GetBillDetails());
        }

        [HttpGet("GetBillDetailsByIDBill/{IDBill}")]
        public async Task<IActionResult> GetBillDetailsByIDBill(int IDBill)
        {
            return Ok(await billDetail.GetBillDetailsByIDBill(IDBill));
        }

        [HttpPost("AddBillDetail")]
        public async Task<ActionResult> AddBillDetail(BillDetailDto billDt)
        {
            var bdt = await billDetail.AddBillDetail(billDt);
            if (bdt == null)
                return BadRequest();
            return Ok(bdt);
        }

        [HttpDelete("RemoveProductBillDetail/{idBillDetail}")]
        public async Task<ActionResult> RemoveProductBillDetail(int idBillDetail)
        {
            var rbd = await billDetail.DeleteProductBillDetail(idBillDetail);
            if (rbd == null)
            {
                return BadRequest();
            }
            return Ok(rbd);
        }
    }
}
