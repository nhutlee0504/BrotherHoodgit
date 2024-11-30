using API.Data;
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
using System.Security.Principal;
using System.Threading.Tasks;

namespace API.Services
{
    public class ProductResponse : IProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        public ProductResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<Product> AddProduct(ProductDto product)
        {
            var user = GetUserInfoFromClaims();

            if (user.UserName == null || user.Email == null || user.FullName == null || user.PhoneNumber == null)
            {
                throw new InvalidOperationException("Thông tin người dùng này là bắt buộc");
            }
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (existingUser == null)
            {
                throw new InvalidOperationException("Người dùng không tồn tại");
            }

            if (existingUser.PreSystem < 5000)
            {
                throw new InvalidOperationException("Số dư không đủ để thực hiện thao tác này");
            }
            existingUser.PreSystem -= 5000;
            _context.Accounts.Update(existingUser);


            var newProd = new Product
            {
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price,
                Description = product.Description,
                IDCategory = product.CategoryId,
                Status = "Đang chờ duyệt",
                ProrityLevel = "Phổ thông",
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                UserName = user.UserName,
                AccountAccept = "Admin"

            };

            await _context.Products.AddAsync(newProd);
            await _context.SaveChangesAsync();

            return newProd;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()//Lấy tất cả sản phẩm
        {
            var getP = await _context.Products.ToListAsync();
            if (getP == null)
            {
                throw new NotImplementedException("Không có sản phẩm hoặc không tìm thấy sản phẩm của bạn");
            }
            return getP;
        }

        public async Task<IEnumerable<Product>> GetProductsAccount()//Lấy tất cả danh sách sản phẩm của người đăng nhập
        {
            var user = GetUserInfoFromClaims();
            var getP = await _context.Products.Where(x => x.UserName == user.UserName).ToListAsync();
            if (getP == null)
            {
                throw new NotImplementedException("Không có sản phẩm hoặc không tìm thấy sản phẩm của bạn");
            }
            return getP;
        }

        public async Task<IEnumerable<Product>> GetProductByNameAccount(string username)
        {
            var products = await _context.Products
                .Include(p => p.imageProducts) // Bao gồm thông tin hình ảnh
                .Include(p => p.Category) // Bao gồm thông tin danh mục
                .Where(p => p.UserName == username)
                .ToListAsync();

            return products.Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price,
                UserName = p.UserName,
                Status = p.Status,
                StartDate = p.StartDate,
                imageProducts = p.imageProducts ?? new List<ImageProduct>(), // Đảm bảo không null
                IDCategory = p.IDCategory // Lấy thông tin danh mục
            }).ToList();
        }

        public async Task<Product> DeleteProductById(int id)//Xóa sản phẩm
        {
            var user = GetUserInfoFromClaims();
            if (user.UserName == null || user.Email == null || user.FullName == null || user.PhoneNumber == null || user.IDCard == null)
            {
                throw new InvalidOperationException("Thông tin người dùng này là bắt buộc");
            }
            var product = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == id);
            if (product == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm");
            }
            if (product.UserName != user.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa sản phẩm này");
            }
            product.Status = "Đã xóa";
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProductById(int id) // Xem chi tiết sản phẩm
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.IDProduct == id);

            if (product == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm tương ứng");
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)//Tìm sản phẩm theo tên
        {
            var GetName = await _context.Products.Where(x => x.Name.Contains(name)).ToListAsync();
            if (GetName == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm tương ứng");
            }
            return GetName;
        }

        public async Task<Product> UpdateProductById(int id, ProductDto product)//Cập nhật sản phẩm có kèm ảnh sản phẩm
        {
            var user = GetUserInfoFromClaims();
            var existingProduct = await _context.Products.Include(p => p.imageProducts).FirstOrDefaultAsync(x => x.IDProduct == id);
            if (existingProduct == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm tương ứng");
            }
            if (existingProduct.UserName != user.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền thay đổi thông tin sản phẩm");
            }
            existingProduct.Name = product.Name;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.IDCategory = product.CategoryId;

            await _context.SaveChangesAsync(); // Save changes for the updated product
            return existingProduct;
        }

        public async Task<Product> AcceptProduct(int idproduct)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == idproduct);
            if (existingProduct == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm tương ứng");
            }

            existingProduct.Status = "Đã duyệt";
            _context.SaveChanges();
            return existingProduct;
        }
        public async Task<Product> CancleProduct(int idproduct)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == idproduct);
            if (existingProduct == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm tương ứng");
            }

            existingProduct.Status = "Đã hủy";
            _context.SaveChanges();
            return existingProduct;
        }

        public async Task<IEnumerable<dynamic>> GetStatisticsByStatusAsync()
        {
            var products = await _context.Products.ToListAsync();

            if (products == null || !products.Any())
            {
                throw new InvalidOperationException("Không có sản phẩm nào để thống kê.");
            }

            var statistics = products
                .GroupBy(p => p.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .ToList();

            return statistics;
        }

        //Phương thức ngooài

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
