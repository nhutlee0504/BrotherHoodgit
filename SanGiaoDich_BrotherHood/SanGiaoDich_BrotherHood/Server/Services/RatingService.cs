
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class RatingService : IRating
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        private readonly string _imagePath;

        public RatingService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhSanPham");
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<Rating> AddRating(int billDetailId, int star, string comment, string image)
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            // Kiểm tra xem BillDetail có tồn tại không
            var bdt = await _context.BillDetails
                .Include(bd => bd.Bill) // Bao gồm thông tin Bill để kiểm tra người dùng
                .FirstOrDefaultAsync(x => x.IDBillDetail == billDetailId);

            if (bdt == null)
            {
                throw new NotImplementedException("Không tìm thấy thông tin sản phẩm đã mua.");
            }

            // Kiểm tra xem người dùng có phải là người đã mua sản phẩm không
            if (bdt.Bill.UserName != userInfo.UserName)
            {
                throw new InvalidOperationException("Bạn không thể đánh giá sản phẩm này vì bạn không phải là người đã mua.");
            }

            // Kiểm tra xem người dùng có thể đánh giá không
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.IDBillDetail == billDetailId && r.UserName == userInfo.UserName);

            if (existingRating != null)
            {
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi.");
            }

            // Tạo và thêm đánh giá mới
            var rating = new Rating
            {
                IDBillDetail = billDetailId,
                Star = star,
                Comment = comment,
                UserName = userInfo.UserName,
                Image = image
                // Xử lý file ảnh nếu cần (giả sử bạn đã xử lý file ảnh)
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return rating;
        }

        public async Task<IEnumerable<RatingDto>> GetRatingUser(string username)
        {
            // Tìm tất cả sản phẩm thuộc về người dùng
            var products = await _context.Products
                .Where(x => x.UserName == username)
                .ToListAsync();

            if (products == null || !products.Any())
            {
                throw new Exception("Người dùng không có sản phẩm nào.");
            }

            // Lấy tất cả IDProduct từ danh sách sản phẩm
            var productIds = products.Select(p => p.IDProduct).ToList();

            // Tìm tất cả các BillDetail liên quan đến danh sách sản phẩm
            var billDetails = await _context.BillDetails
                .Where(x => productIds.Contains(x.IDProduct))
                .ToListAsync();

            if (billDetails == null || !billDetails.Any())
            {
                throw new Exception("Không tìm thấy chi tiết hóa đơn liên quan.");
            }

            // Lấy tất cả IDProduct từ BillDetails
            var billDetailIdsP = billDetails.Select(x => x.IDProduct).ToList();

            // Lấy ảnh sản phẩm từ bảng ImageProducts, đảm bảo lấy ảnh chính
            var productImages = await _context.ImageProducts
                .Where(ip => billDetailIdsP.Contains(ip.IDProduct))
                .ToListAsync();

            // Lấy tất cả các đánh giá liên quan đến danh sách BillDetails
            var ratings = await _context.Ratings
                .Where(r => billDetails.Select(bd => bd.IDBillDetail).Contains(r.IDBillDetail))
                .Join(
                    _context.Accounts, // Tham gia với bảng Users
                    r => r.UserName,
                    u => u.UserName,
                    (r, u) => new { r, u }
                )
                .ToListAsync();

            // Kết hợp thông tin đánh giá, ảnh sản phẩm và tên sản phẩm
            var result = ratings.Select(x =>
            {
                // Lấy ảnh sản phẩm từ bảng ImageProducts dựa trên IDProduct của BillDetail
                var productImage = productImages
                    .FirstOrDefault(pi => pi.IDProduct == billDetails
                        .FirstOrDefault(bd => bd.IDBillDetail == x.r.IDBillDetail)?.IDProduct)?.Image;

                // Lấy tên sản phẩm từ bảng Products dựa trên IDProduct trong BillDetails
                var productName = products
                    .FirstOrDefault(p => p.IDProduct == billDetails
                        .FirstOrDefault(bd => bd.IDBillDetail == x.r.IDBillDetail)?.IDProduct)?.Name;

                return new RatingDto
                {
                    Star = x.r.Star,
                    Comment = x.r.Comment,
                    Image = x.r.Image,
                    UserName = x.r.UserName,
                    FullName = x.u.FullName,
                    ImageAccount = x.u.ImageAccount,
                    ProductImage = productImage, // Thêm ảnh sản phẩm vào RatingDto
                    ProductName = productName // Thêm tên sản phẩm vào RatingDto
                };
            }).ToList();

            return result;
        }

        public async Task<bool> IsProductRated(int billDetailId)
        {
            return await _context.Ratings.AnyAsync(r => r.IDBillDetail == billDetailId);
        }


        public async Task<IEnumerable<RatingDto>> GetRatings(int productId)
        {
            return await _context.Ratings
                .Include(r => r.Account)
                .Include(r => r.BillDetail)
                    .ThenInclude(bd => bd.Product)
                .Where(r => r.BillDetail.Product.IDProduct == productId)
                .Select(r => new RatingDto
                {
                    Star = r.Star,
                    Comment = r.Comment,
                    Image = r.Image,
                    UserName = r.Account.UserName // Giả sử UserName là trường bạn muốn lấy
                })
                .ToListAsync();
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
