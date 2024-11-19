using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class ImageProductResponse : IImageProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly string _imagePath;

        public ImageProductResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhSanPham");
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task DeleteImage(int idProd, int idImage)
        {
            var user = GetUserInfoFromClaims();
            var product = await _context.Products.FindAsync(idProd);

            if (product == null)
                throw new KeyNotFoundException("Sản phẩm không tồn tại.");

            if (product.UserName != user.UserName)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa ảnh này.");

            var imageProduct = await _context.ImageProducts.FindAsync(idImage);
            if (imageProduct != null)
            {
                var imagePath = Path.Combine(_imagePath, imageProduct.Image);
                if (File.Exists(imagePath))
                    File.Delete(imagePath);

                _context.ImageProducts.Remove(imageProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Ảnh không tồn tại.");
            }
        }

        public async Task<IEnumerable<ImageProduct>> GetImageProducts(int id)
        {
            var images = await _context.ImageProducts.Where(ip => ip.IDProduct == id).ToListAsync();
            if (images == null || !images.Any())
                throw new KeyNotFoundException("Sản phẩm không có ảnh.");

            return images;
        }

        public async Task<IEnumerable<ImageProduct>> UploadImages(List<IFormFile> files, int productId)
        {
            var user = GetUserInfoFromClaims();
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("Sản phẩm không tồn tại.");

            if (product.UserName != user.UserName)
                throw new UnauthorizedAccessException("Bạn không có quyền thêm ảnh cho sản phẩm này.");

            if (!Directory.Exists(_imagePath))
                Directory.CreateDirectory(_imagePath);

            var imageProducts = new List<ImageProduct>();
            if (files.Count > 3)
                throw new InvalidOperationException("Bạn chỉ được chọn tối đa 3 ảnh.");

            foreach (var file in files)
            {
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(_imagePath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageProduct = new ImageProduct
                {
                    Image = uniqueFileName,
                    IDProduct = productId
                };

                imageProducts.Add(await AddImage(imageProduct));
            }

            return imageProducts;
        }

        public async Task<ImageProduct> UpdateImage(int imageId, IFormFile newFile)
        {
            var user = GetUserInfoFromClaims();
            var imageProduct = await _context.ImageProducts.FindAsync(imageId);

            if (imageProduct == null)
                throw new KeyNotFoundException("Ảnh không tồn tại.");

            var product = await _context.Products.FindAsync(imageProduct.IDProduct);
            if (product == null || product.UserName != user.UserName)
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật ảnh này.");

            if (!Directory.Exists(_imagePath))
                Directory.CreateDirectory(_imagePath);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(newFile.FileName)}";
            var newFilePath = Path.Combine(_imagePath, uniqueFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await newFile.CopyToAsync(stream);
            }

            var oldFilePath = Path.Combine(_imagePath, imageProduct.Image);
            if (File.Exists(oldFilePath))
                File.Delete(oldFilePath);

            imageProduct.Image = uniqueFileName;
            _context.ImageProducts.Update(imageProduct);
            await _context.SaveChangesAsync();

            return imageProduct;
        }

        public async Task<IEnumerable<ImageProduct>> GetImages()
        {
            return await _context.ImageProducts.ToListAsync();
        }

        public async Task<ImageProduct> AddImage(ImageProduct imageProduct)
        {
            _context.ImageProducts.Add(imageProduct);
            await _context.SaveChangesAsync();
            return imageProduct;
        }

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
                if (!string.IsNullOrWhiteSpace(birthdayClaim?.Value) && DateTime.TryParse(birthdayClaim.Value, out DateTime parsedBirthday))
                    birthday = parsedBirthday;

                DateTime? timeBanned = null;
                if (!string.IsNullOrWhiteSpace(timeBannedClaim?.Value) && DateTime.TryParse(timeBannedClaim.Value, out DateTime parsedTimeBanned))
                    timeBanned = parsedTimeBanned;

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
                    timeBanned
                );
            }
            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }
    }
}
