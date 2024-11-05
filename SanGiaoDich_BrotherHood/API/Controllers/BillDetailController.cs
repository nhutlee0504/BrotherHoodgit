using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
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

        [HttpGet("IDBill")]
        public async Task<ActionResult> GetBillDetailsByIDBill(int IDBill)
        {
            return Ok(await billDetail.GetBillDetailsByIDBill(IDBill));
        }

        [HttpPost]
        public async Task<ActionResult> AddBillDetail(BillDetail billDt)
        {
            var bdt = await billDetail.AddBillDetail(new BillDetail
            {
                IDBill = billDt.IDBill,
                IDProduct = billDt.IDProduct,
                Quantity = billDt.Quantity,
                Price = billDt.Price
            });
            if (bdt == null)
                return BadRequest();
            return CreatedAtAction("AddBillDetail", bdt);
        }
    }
}
