using API.Dto;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Threading.Tasks;

namespace API.Controllers
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

        [HttpGet("GetFavoriteAccount")]
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

        [HttpGet("AddFavorite")]
        public async Task<IActionResult> AddFavorite(int idProd)
        {
            try
            {
                var newFav = await favorite.AddFavorite(idProd);
                return Ok(newFav);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteFavorite")]
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
