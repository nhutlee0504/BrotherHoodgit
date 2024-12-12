
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class CartResponse : ICart
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
		public async Task<CartItem> AddCart(int idProduct)
		{
			try
			{
				var user = GetUserInfoFromClaims();
				// Tìm giỏ hàng của người dùng
				var cartFind = await _context.Carts.FirstOrDefaultAsync(u => u.UserName == user.UserName);

				if (cartFind != null)
				{
					// Kiểm tra nếu sản phẩm đã có trong giỏ hàng
					var existingItem = await _context.CartItems
						.FirstOrDefaultAsync(ci => ci.IDCart == cartFind.IDCart && ci.IDProduct == idProduct);

					if (existingItem != null)
					{
						// Nếu sản phẩm đã có trong giỏ hàng, ném lỗi
						throw new InvalidOperationException("Sản phẩm đã có trong giỏ hàng.");
					}

					// Thêm sản phẩm vào giỏ hàng
					var newCartItem = new CartItem
					{
						IDCart = cartFind.IDCart,
						IDProduct = idProduct,
						CreatedDate = DateTime.Now,
					};
					_context.CartItems.Add(newCartItem);
					await _context.SaveChangesAsync();
					return newCartItem;
				}

				// Nếu không tìm thấy giỏ hàng của người dùng, ném lỗi
				throw new NotFiniteNumberException("Không tìm thấy giỏ hàng của người dùng.");
			}
			catch (Exception ex)
			{
				// Có thể tùy chỉnh cách xử lý lỗi tại đây, chẳng hạn trả về null hoặc xử lý cụ thể
				return null;
			}
		}

		public async Task<Cart> DeleteCart(int IDCart)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(IDCart);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<Cart> GetCartsByUserName(string userName)
        {
            return await _context.Carts.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<Cart> UpdateCart(int IDCart, Cart cart)
        {
            try
            {
                var cartUpdate = await _context.Carts.FindAsync(IDCart);
                if (cartUpdate == null)
                    return null;
                cartUpdate.UserName = cart.UserName;

                _context.Carts.Update(cartUpdate);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        private (string UserName, string Email, string FullName, string PhoneNumber, string Gender, string IDCard, DateTime? Birthday, string ImageAccount, string Role, bool IsDelete, DateTime? TimeBanned) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                // Kiểm tra thời gian hết hạn của token
                var expirationClaim = userClaim.FindFirst("exp");
                if (expirationClaim != null && long.TryParse(expirationClaim.Value, out long exp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    if (expirationTime < DateTime.UtcNow)
                    {
                        throw new UnauthorizedAccessException("Token đã hết hạn. Vui lòng đăng nhập lại.");
                    }
                }

                var userNameClaim = userClaim.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                var fullNameClaim = userClaim.FindFirst("FullName")?.Value ?? string.Empty;
                var phoneNumberClaim = userClaim.FindFirst("PhoneNumber")?.Value ?? string.Empty;
                var genderClaim = userClaim.FindFirst("Gender")?.Value ?? string.Empty;
                var idCardClaim = userClaim.FindFirst("IDCard")?.Value ?? string.Empty;
                var imageAccountClaim = userClaim.FindFirst("ImageAccount")?.Value ?? string.Empty;
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

                DateTime? birthday = null;
                var birthdayClaimValue = userClaim.FindFirst("Birthday")?.Value;
                if (!string.IsNullOrWhiteSpace(birthdayClaimValue) && DateTime.TryParse(birthdayClaimValue, out DateTime parsedBirthday))
                {
                    birthday = parsedBirthday;
                }

                bool isDelete = false;
                var isDeleteClaimValue = userClaim.FindFirst("IsDelete")?.Value;
                if (isDeleteClaimValue != null && bool.TryParse(isDeleteClaimValue, out bool parsedIsDeleted))
                {
                    isDelete = parsedIsDeleted;
                }

                DateTime? timeBanned = null;
                var timeBannedClaimValue = userClaim.FindFirst("TimeBanned")?.Value;
                if (!string.IsNullOrWhiteSpace(timeBannedClaimValue) && DateTime.TryParse(timeBannedClaimValue, out DateTime parsedTimeBanned))
                {
                    timeBanned = parsedTimeBanned;
                }

                return (
                    userNameClaim,
                    emailClaim,
                    fullNameClaim,
                    phoneNumberClaim,
                    genderClaim,
                    idCardClaim,
                    birthday,
                    imageAccountClaim,
                   roleClaim,
                   isDelete,
                   timeBanned
                );
            }

            throw new UnauthorizedAccessException("Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại.");
        }
    }
}
