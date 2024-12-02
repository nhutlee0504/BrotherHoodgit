using API.Dto;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItem _cartItemService;

        public CartItemController(ICartItem cartItemService)
        {
            _cartItemService = cartItemService;
        }

        // Thêm nhiều sản phẩm vào giỏ hàng
        [HttpPost("add-many")]
        public async Task<IActionResult> AddMultipleCartItems([FromBody] List<CartItemDto> cartItemDtos)
        {
            if (cartItemDtos == null || cartItemDtos.Count == 0)
            {
                return BadRequest(new { Message = "Dữ liệu không hợp lệ. Vui lòng cung cấp danh sách sản phẩm." });
            }

            try
            {
                // Lưu các sản phẩm đã thêm thành công
                var addedItems = new List<CartItemDto>();

                foreach (var cartItemDto in cartItemDtos)
                {
                    var result = await _cartItemService.AddCartItem(cartItemDto);

                    if (result != null)
                    {
                        addedItems.Add(cartItemDto);  // Thêm sản phẩm vào danh sách đã thêm
                    }
                }

                if (addedItems.Count == 0)
                {
                    return NotFound(new { Message = "Không có sản phẩm nào được thêm vào giỏ hàng." });
                }

                return Ok(addedItems);  // Trả về danh sách sản phẩm đã thêm vào giỏ hàng
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi thêm các sản phẩm vào giỏ hàng.", Error = ex.Message });
            }
        }

        // Lấy tất cả sản phẩm trong giỏ hàng
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCartItems([FromQuery] int idCart)
        {
            try
            {
                var result = await _cartItemService.GetAllCartItems(idCart);

                if (result == null)
                {
                    return NotFound(new { Message = "Giỏ hàng hiện tại chưa có sản phẩm." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách sản phẩm trong giỏ hàng.", Error = ex.Message });
            }
        }


        [HttpDelete("delete/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            try
            {
                var result = await _cartItemService.DeleteCartItem(cartItemId);

                if (!result)
                {
                    return NotFound(new { Message = "Không tìm thấy sản phẩm trong giỏ hàng để xóa." });
                }

                return Ok(new { Message = "Sản phẩm đã được xóa khỏi giỏ hàng." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa sản phẩm khỏi giỏ hàng.", Error = ex.Message });
            }
        }
    }
}
