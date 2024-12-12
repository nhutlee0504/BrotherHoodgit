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
        private readonly FirebaseStorageService _firebaseService;
        public ImageProductResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, FirebaseStorageService firebaseService)
        {
            _firebaseService = firebaseService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task DeleteImage(int idProd, int idImage)
        {
            var user = GetUserInfoFromClaims();

            var product = await _context.Products.FindAsync(idProd);

            if (product == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại.");
            }
            if (product.UserName != user.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa ảnh này.");
            }
            var imageProduct = await _context.ImageProducts.FindAsync(idImage);

            if (imageProduct != null)
            {
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
            var Image = await _context.ImageProducts.Where(ip => ip.IDProduct == id).ToListAsync();
            if (Image == null)
            {
                throw new NotImplementedException("Sản phẩm không có ảnh");
            }
            return Image;
        }

        public async Task<IEnumerable<ImageProduct>> UploadImages(List<IFormFile> files, int productId)
        {
            var user = GetUserInfoFromClaims();
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException("Sản phẩm không tồn tại.");
            }

            if (product.UserName != user.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thêm ảnh cho sản phẩm này.");
            }

            var imageProducts = new List<ImageProduct>();
            foreach (var file in files)
            {
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = file.OpenReadStream())
                {
                    try
                    {
                        var downloadUrl = await _firebaseService.UploadFileToFirebaseStorage(stream, uniqueFileName);
                        var imageProduct = new ImageProduct
                        {
                            Image = downloadUrl,
                            IDProduct = productId
                        };

                        imageProducts.Add(await AddImage(imageProduct));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Lỗi khi tải ảnh lên Firebase: {ex.Message}");
                    }
                }
            }

            return imageProducts;
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
       // public async Task<ImageProduct> UpdateImage(int imageId, IFormFile newFile)
       // {
       //     var user = GetUserInfoFromClaims();
       //     var imageProduct = await _context.ImageProducts.FindAsync(imageId);

       //     if (imageProduct == null)
       //         throw new KeyNotFoundException("Ảnh không tồn tại.");

       //     var product = await _context.Products.FindAsync(imageProduct.IDProduct);
       //     if (product == null || product.UserName != user.UserName)
       //         throw new UnauthorizedAccessException("Bạn không có quyền cập nhật ảnh này.");

       //if (!Directory.Exists(_imagePath))
       //         Directory.CreateDirectory(_imagePath);

       //     var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(newFile.FileName)}";
       //     var newFilePath = Path.Combine(_imagePath, uniqueFileName);

       //     using (var stream = new FileStream(newFilePath, FileMode.Create))
       //     {
       //         await newFile.CopyToAsync(stream);
       //     }

       //     var oldFilePath = Path.Combine(_imagePath, imageProduct.Image);
       //     if (File.Exists(oldFilePath))
       //         File.Delete(oldFilePath);

       //     imageProduct.Image = uniqueFileName;
       //     _context.ImageProducts.Update(imageProduct);
       //     await _context.SaveChangesAsync();

       //     return imageProduct;
       // }
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

        public Task<ImageProduct> UpdateImage(int idImage, IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}