
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using System.Collections.Generic;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Linq;
using SanGiaoDich_BrotherHood.Server.Dto;

namespace SanGiaoDich_BrotherHood.Server.Controllers
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
        public async Task<IActionResult> RateProduct(int billDetailId, int star, string comment, string image)
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

		[HttpGet("GetRatingsUser{username}")]
		public async Task<ActionResult> GetRatingsUser(string username)
		{
			try
			{
				var ratings = await _ratingService.GetRatingUser(username);
				if (ratings == null || !ratings.Any())
				{
					return NotFound("Không tìm thấy đánh giá cho sản phẩm này.");
				}

				// Chuyển đổi sang RatingDto trước khi trả về client
				var ratingDtos = ratings.Select(r => new RatingDto
				{
					Star = r.Star,
					Comment = r.Comment,
					Image = r.Image,
					UserName = r.UserName,
                    FullName = r.FullName,
                    ImageAccount = r.ImageAccount,
                    ProductImage = r.ProductImage,
                    ProductName = r.ProductName,
				}).ToList();

				return Ok(ratingDtos);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Lỗi server: {ex.Message}");
			}
		}

        [HttpGet("IsProductRated")]
        public async Task<IActionResult> IsProductRated(int billDetailId)
        {
            try
            {
                var isRated = await _ratingService.IsProductRated(billDetailId);
                return Ok(isRated);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
