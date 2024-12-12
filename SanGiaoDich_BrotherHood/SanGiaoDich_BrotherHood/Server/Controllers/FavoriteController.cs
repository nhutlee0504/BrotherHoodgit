using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Server.Dto;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavorite favorite;
        public FavoriteController(IFavorite favorite)
        {
            this.favorite = favorite;
        }

        [HttpGet]
        [Route("GetFavoriteAccount")]
        public async Task<IActionResult> GetFavoriteAccount()
        {
            try
            {
                var fav = await favorite.GetFavoritesAccount();
                return Ok(fav);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("AddFavorite/{idProd}")]
        public async Task<IActionResult> AddFavorite(int idProd)
        {
            try
            {
                var newFav = await favorite.AddFavorite(idProd);
                return Ok(newFav);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);  // Nếu không có quyền (chưa đăng nhập)
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");  // Lỗi server
            }
        }


        [HttpDelete]
        [Route("DeleteFavorite/{IdProd}")]
        public async Task<IActionResult> DeleteFavorite(int IdProd)
        {
            try
            {
                var delFav = await favorite.DeleteFavorite(IdProd);
                return Ok(delFav);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
