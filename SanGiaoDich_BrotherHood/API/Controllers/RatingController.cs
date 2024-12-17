using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using API.Models;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRating _ratingService;

        public RatingController(IRating ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpGet("GetAllRating")]
        public async Task<ActionResult> GetRatings(int productId)
        {
            try
            {
                var ratings = await _ratingService.GetRatings(productId);
                if (ratings == null)
                {
                    return NotFound("Không tìm thấy đánh giá cho sản phẩm này.");
                }

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }



        [HttpPost("AddRating")]
        public async Task<IActionResult> RateProduct(int billDetailId, int star, string comment, IFormFile image)
        {
            try
            {
                var rating = await _ratingService.AddRating(billDetailId, star, comment, image);
                return Ok(rating);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Trả về mã lỗi 409 nếu đã đánh giá rồi
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Có lỗi xảy ra: {ex.Message}");
            }
        }

		[HttpGet("{username}")]
		public async Task<ActionResult> GetRatingsUser(string username)
		{
			try
			{
				var ratings = await _ratingService.GetRatingUser(username);
				if (ratings == null)
				{
					return NotFound("Không tìm thấy đánh giá cho sản phẩm này.");
				}

				return Ok(ratings);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi server: {ex.Message}");
			}
		}
	}
}
