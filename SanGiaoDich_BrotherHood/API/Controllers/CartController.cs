using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICart cart;
        public CartController(ICart cart)
        {
            this.cart = cart;
        }
        [HttpGet("userName")]
        public async Task<ActionResult> GetCartsByUserName([FromQuery] string userName)
        {
            var result = await cart.GetCartsByUserName(userName);
            if (result == null)
            {
                return NotFound(new { Message = "Không tìm thấy giỏ hàng của người dùng." });
            }
            return Ok(result);
        }


        [HttpPut("IDCart")]
        public async Task<ActionResult> UpdateCart(int IDCart, Cart c)
        {
            return Ok(await cart.UpdateCart(IDCart, c));
        }

        [HttpDelete("IDCart")]
        public async Task<ActionResult> DeleteCart(int IDCart)
        {
            var ca = await cart.DeleteCart(IDCart);
            if (ca == null)
                return BadRequest();
            return NoContent();
        }
    }
}
