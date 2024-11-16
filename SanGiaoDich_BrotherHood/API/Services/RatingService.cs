﻿using API.Data;
using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Services
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

        public async Task<Rating> AddRating(int billDetailId, int star, string comment, IFormFile image)//Thêm đánh giá
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
                // Xử lý file ảnh nếu cần (giả sử bạn đã xử lý file ảnh)
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return rating;
        }


        public async Task<IEnumerable<RatingDto>> GetRatings(int productId)//Lấy tất cả đánh giá
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
