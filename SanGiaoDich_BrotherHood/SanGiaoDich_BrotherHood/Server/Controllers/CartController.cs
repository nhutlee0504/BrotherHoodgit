
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Shared.Models;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
        [HttpGet]
        [Route("GetCartsByUserName/{userName}")]
        public async Task<ActionResult> GetCartsByUserName(string userName)
        {
            var result = await cart.GetCartsByUserName(userName);
            if (result == null)
            {
                return NotFound(new { Message = "Không tìm thấy giỏ hàng của người dùng." });
            }
            return Ok(result);
        }

        [HttpPost("AddCart/{idProduct}")]
        public async Task<ActionResult> AddCart(int idProduct)
        {
            try
            {
                var add = await cart.AddCart(idProduct);
                return Ok(add);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
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
