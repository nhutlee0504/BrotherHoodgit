
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressDetailController : ControllerBase
    {
        private IAddressDetail address;
        public AddressDetailController(IAddressDetail address)
        {
            this.address = address;
        }

        [HttpGet]
        public async Task<ActionResult> GetAddressDetails()
        {
            return Ok(await address.GetAddressDetails());
        }


        [HttpGet]
        [Route("GetAddressDetailsByUserName/{username}")]
        public async Task<ActionResult> GetAddressDetailsByUserName(string userName)
        {
            return Ok(await address.GetAddressDetailsByUserName(userName));
        }

        [HttpPost]
        [Route("AddAddress")]
        public async Task<ActionResult> AddAddress(AddressDetail addressDetail)
        {
            var ar = await address.AddAddress(new AddressDetail
            {
                ProvinceCity = addressDetail.ProvinceCity,
                District = addressDetail.District,
                Wardcommune = addressDetail.Wardcommune,
                AdditionalDetail = addressDetail.AdditionalDetail,
                UserName = addressDetail.UserName
            });
            if (ar == null)
                return BadRequest();
            return CreatedAtAction("AddAddress", ar);
        }

        [HttpPut("IDAdress")]
        public async Task<ActionResult> UpdateAddress(int IDAddress, AddressDetail addressDetail)
        {
            return Ok(await address.UpdateAddress(IDAddress, addressDetail));
        }

        [HttpDelete("IDAdress")]
        public async Task<ActionResult> DeleteAddress(int IDAddress)
        {
            var ar = await address.DeleteAddress(IDAddress);
            if (ar == null)
                return BadRequest();
            return NoContent();
        }
    }
}
