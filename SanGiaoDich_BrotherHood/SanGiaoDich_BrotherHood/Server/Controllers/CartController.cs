
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Controllers
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
        public async Task<ActionResult> GetCartsByUserName(string userName)
        {
            return Ok(await cart.GetCartsByUserName(userName));
        }

        [HttpPost]
        public async Task<ActionResult> AddCart(Cart c)
        {
            var ca = await cart.AddCart(new Cart
            {
                UserName = c.UserName,
                IDProduct = c.IDProduct,
                Quantity = c.Quantity
            });
            if (ca == null)
                return BadRequest();
            return CreatedAtAction("AddCart", ca);
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
