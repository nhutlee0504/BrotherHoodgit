using API.Data;
using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Services
{
    public class FavoriteResponse : IFavorite
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        public FavoriteResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<Favorite> AddFavorite(int idProd)
        {
            var user = GetUserInfoFromClaims();
            var newFav = new Favorite
            {
                UserName = user.UserName,
                IDFavorite = idProd,
                IDProduct = idProd,
                CreatedDate = DateTime.Now
            };
            await _context.Favorites.AddAsync(newFav);
            await _context.SaveChangesAsync();
            return newFav;
        }

        public async Task<Favorite> DeleteFavorite(int idprod)//Xóa sản phẩm yêu thích của người dùng
        {
            var user = GetUserInfoFromClaims();
            var deFav = await _context.Favorites.FirstOrDefaultAsync(x => x.IDProduct == idprod && x.UserName == user.UserName);
            if (deFav != null)
            {
                _context.Remove(deFav);
                await _context.SaveChangesAsync();
                return deFav;
            }
            throw new NotImplementedException("Không tìm thấy sản phẩm trong danh sách của bạn");
        }

        public async Task<IEnumerable<Favorite>> GetFavoritesAccount()//Lấy tất cả danh sách yêu thích của người dùng
        {
            var user = GetUserInfoFromClaims();
            var getFav = await _context.Favorites.Where(x => x.UserName == user.UserName).ToListAsync();
            if (getFav != null)
            {
                return getFav;
            }
            throw new NotImplementedException("Không có sản phẩm trong danh sách yêu thích của bạn");
        }

        //Phương thức ngoài
        private (string UserName, string Email, string FullName, string PhoneNumber, string Gender, string IDCard, DateTime? Birthday, string ImageAccount, string Role, bool IsDelete, DateTime? TimeBanned) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var userNameClaim = userClaim.FindFirst(ClaimTypes.Name);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var fullNameClaim = userClaim.FindFirst("FullName");
                var phoneNumberClaim = userClaim.FindFirst("PhoneNumber");
                var genderClaim = userClaim.FindFirst("Gender");
                var idCardClaim = userClaim.FindFirst("IDCard");
                var birthdayClaim = userClaim.FindFirst("Birthday");
                var imageAccountClaim = userClaim.FindFirst("ImageAccount");
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);
                var isDeleteClaim = userClaim.FindFirst("IsDelete");
                var timeBannedClaim = userClaim.FindFirst("TimeBanned");

                DateTime? birthday = null;
                if (!string.IsNullOrWhiteSpace(birthdayClaim?.Value))
                {
                    if (DateTime.TryParse(birthdayClaim.Value, out DateTime parsedBirthday))
                    {
                        birthday = parsedBirthday;
                    }
                    else
                    {
                        // Log or handle the invalid date format here if needed
                    }
                }

                return (
                    userNameClaim?.Value,
                    emailClaim?.Value,
                    fullNameClaim?.Value,
                    phoneNumberClaim?.Value,
                    genderClaim?.Value,
                    idCardClaim?.Value,
                    birthday,
                    imageAccountClaim?.Value,
                    roleClaim?.Value,
                    isDeleteClaim != null && bool.TryParse(isDeleteClaim.Value, out bool isDeleted) && isDeleted,
                    timeBannedClaim != null ? DateTime.TryParse(timeBannedClaim.Value, out DateTime parsedTimeBanned) ? parsedTimeBanned : (DateTime?)null : (DateTime?)null
                );
            }
            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }

    }
}
