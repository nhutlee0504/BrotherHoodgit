
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
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class ProductResponse : IProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        private readonly HttpClient _httpClient;
		private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta2/models/gemini-1.5-safe/contentModeration"; // Địa chỉ API kiểm duyệt văn bản
		private readonly string _apiKey = "AIzaSyDbce3o_4id0lVOhGr0Xva0KJcVeR5RRc4"; // Thay API key của bạn

		public ProductResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClient = httpClient;
        }

		public async Task<bool> ContainsProfanityAsync(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentException("Nội dung kiểm tra không được để trống.");

			var requestPayload = new { text };
			var requestContent = new StringContent(
				JsonSerializer.Serialize(requestPayload),
				Encoding.UTF8,
				"application/json"
			);

			// Sử dụng HttpRequestMessage để thêm header Authorization
			var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl)
			{
				Content = requestContent
			};

			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

			try
			{
				var response = await _httpClient.SendAsync(request);

				if (!response.IsSuccessStatusCode)
				{
					var errorDetails = await response.Content.ReadAsStringAsync();
					throw new InvalidOperationException($"Lỗi từ API kiểm duyệt: {response.StatusCode} - {errorDetails}");
				}

				var result = await response.Content.ReadAsStringAsync();
				var profanityResult = JsonSerializer.Deserialize<ProfanityCheckResult>(result);

				if (profanityResult == null)
					throw new InvalidOperationException("API trả về phản hồi không hợp lệ hoặc null.");

				return profanityResult.IsProfane;
			}
			catch (HttpRequestException ex)
			{
				throw new InvalidOperationException($"Lỗi kết nối đến API kiểm duyệt: {ex.Message}");
			}
			catch (JsonException ex)
			{
				throw new InvalidOperationException($"Lỗi giải mã JSON từ phản hồi API: {ex.Message}");
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Lỗi không xác định: {ex.Message}");
			}
		}


		public class ProfanityCheckResult
		{
			public bool IsProfane { get; set; }
			public string Reason { get; set; } // Nếu API cung cấp lý do (optional)
		}


		// Định nghĩa model phản hồi của API kiểm duyệt
		private class ModerationResponse
		{
			public bool IsSafe { get; set; }
			public string Reason { get; set; }
		}

		public async Task<Product> AddProduct(ProductDto product)
		{

			// Phần còn lại của logic thêm sản phẩm
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

			int deductionAmount;
			if (product.ProrityLevel == "Ưu tiên")
			{
				deductionAmount = 50000; // Mức trừ cho sản phẩm ưu tiên
			}
			else if (product.ProrityLevel == "Phổ thông")
			{
				deductionAmount = 25000; // Mức trừ cho sản phẩm phổ thông
			}
			else
			{
				throw new InvalidOperationException("Mức độ ưu tiên không hợp lệ");
			}

			// Kiểm tra số dư
			if (existingUser.PreSystem < deductionAmount)
			{
				throw new InvalidOperationException("Số dư không đủ để thực hiện thao tác này");
			}

			// Trừ số dư
			existingUser.PreSystem -= deductionAmount;
			_context.Accounts.Update(existingUser);

			var newProd = new Product
			{
				Name = product.Name,
				Quantity = product.Quantity,
				Price = product.Price,
				Description = product.Description,
				IDCategory = product.CategoryId,
				Status = "Đang chờ duyệt",
				ProrityLevel = product.ProrityLevel,
				CreatedDate = DateTime.Now,
				UpdatedDate = DateTime.Now,
				StartDate = DateTime.Now,
				UserName = user.UserName,
				AccountAccept = "Admin"
			};

			await _context.Products.AddAsync(newProd);
			await _context.SaveChangesAsync();

			return newProd;
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()//Lấy tất cả sản phẩm
        {
            var getP = await _context.Products.OrderByDescending(cre => cre.CreatedDate).ToListAsync();
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
            //if (user.UserName == null || user.Email == null || user.FullName == null || user.PhoneNumber == null || user.IDCard == null)
            //{
            //    throw new InvalidOperationException("Thông tin người dùng này là bắt buộc");
            //}
            var product = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == id);
            if(product == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm");
            }
            //if(product.UserName != user.UserName)
            //{
            //    throw new UnauthorizedAccessException("Bạn không có quyền xóa sản phẩm này");
            //}
            product.Status = "Đã xóa";
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetProductById(int id) // Xem chi tiết sản phẩm
        {
            var product = await _context.Products
                .Include(p => p.imageProducts)
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
            if(GetName == null)
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
            if(existingProduct.UserName != user.UserName)
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
            decimal refundAmount = 0;
            if (existingProduct.ProrityLevel == "Phổ thông")
            {
                refundAmount = 25000 * 0.95m; 
            }
            else if (existingProduct.ProrityLevel == "Ưu tiên")
            {
                refundAmount = 50000 * 0.95m;
            }
            else
            {
                throw new NotImplementedException("Priority không hợp lệ");
            }


     
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == existingProduct.UserName);
            if (user == null)
            {
                throw new NotImplementedException("Không tìm thấy người dùng tương ứng");
            }
            user.PreSystem += refundAmount;


            await _context.SaveChangesAsync();

            return existingProduct;
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

		public async Task<Product> UpdateProrityLevel(int id)
		{
			var prodFind = await _context.Products.FindAsync(id);

			if (prodFind == null)
			{
				throw new NotImplementedException("Không tìm thấy sản phẩm");
			}

			if (prodFind.ProrityLevel == "Ưu tiên")
			{
				throw new InvalidOperationException("Sản phẩm đã ở mức ưu tiên");
			}

			// Trừ tiền của người dùng
			var user = GetUserInfoFromClaims();
			var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == user.UserName);

			if (existingUser == null)
			{
				throw new InvalidOperationException("Người dùng không tồn tại");
			}

			const int upgradeCost = 50000; // Giá nâng cấp lên ưu tiên
			if (existingUser.PreSystem < upgradeCost)
			{
				throw new InvalidOperationException("Số dư không đủ để nâng cấp sản phẩm");
			}

			existingUser.PreSystem -= upgradeCost;
			_context.Accounts.Update(existingUser);

			// Cập nhật mức độ ưu tiên
			prodFind.ProrityLevel = "Ưu tiên";
			prodFind.UpdatedDate = DateTime.Now;

			await _context.SaveChangesAsync();
			return prodFind;
		}
	}
      
}
