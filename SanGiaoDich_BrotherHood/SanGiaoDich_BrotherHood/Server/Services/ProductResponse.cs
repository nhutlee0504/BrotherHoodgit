
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
using OfficeOpenXml;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class ProductResponse : IProduct
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        private readonly HttpClient _httpClient;
        private object package;

        public ProductResponse(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, HttpClient httpClient)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClient = httpClient;
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
                IDProduct = p.IDProduct,
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
            if (product == null)
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
        public async Task<IEnumerable<dynamic>> GetStatisticsByStatusAsync()
        {
            var products = await _context.Products.ToListAsync();

            if (products == null || !products.Any())
            {
                throw new InvalidOperationException("Không có sản phẩm nào để thống kê.");
            }

            // Tính tổng số bài đăng
            var totalPosts = products.Count();

            // Thống kê theo trạng thái
            var statistics = products
                .GroupBy(p => p.Status)
                .Select(group => new
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .ToList();

            // Thêm tổng số bài đăng vào đầu danh sách thống kê
            statistics.Insert(0, new
            {
                Status = "Tổng số bài đăng",
                Count = totalPosts
            });

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

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var totalRevenue = await _context.Products
                                              .Where(p => p.Status == "Đã duyệt")
                                              .SumAsync(p => p.Price); // Tổng doanh thu = giá * số lượng
            return totalRevenue;
        }

        public async Task<decimal> GetRevenueByWeekAsync(DateTime startDate)
        {
            // Xác định ngày bắt đầu và ngày kết thúc của tuần
            var startOfWeek = startDate.Date;
            var endOfWeek = startOfWeek.AddDays(7); // Lấy ngày đầu tiên của tuần tiếp theo

            // Kiểm tra nếu không có đơn hàng trong tuần
            var revenue = await _context.Products
                .Where(order => order.CreatedDate >= startOfWeek && order.CreatedDate < endOfWeek)
                .SumAsync(order => (decimal?)order.Price) ?? 0; // Nếu không có giá trị thì trả về 0

            return revenue;
        }

        public async Task<decimal> GetRevenueByMonthAsync(int month, int year)
        {
            // Kiểm tra dữ liệu đầu vào hợp lệ
            if (month < 1 || month > 12 || year < 1)
                throw new ArgumentException("Tháng hoặc năm không hợp lệ.");

            // Xác định ngày bắt đầu và ngày kết thúc của tháng
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1); // Lấy ngày đầu tiên của tháng tiếp theo

            // Kiểm tra nếu không có đơn hàng trong tháng
            var revenue = await _context.Products
                .Where(order => order.CreatedDate >= startOfMonth && order.CreatedDate < endOfMonth)
                .SumAsync(order => (decimal?)order.Price) ?? 0; // Nếu không có giá trị thì trả về 0

            return revenue;
        }

        public async Task<decimal> GetRevenueByDateAsync(DateTime date)
        {
            // Xác định ngày bắt đầu và ngày kết thúc của ngày
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1); // Lấy ngày tiếp theo để so sánh

            // Kiểm tra nếu không có đơn hàng trong ngày
            var revenue = await _context.Products
                .Where(order => order.CreatedDate >= startOfDay && order.CreatedDate < endOfDay && order.Status.Contains("Đã duyệt"))
                .SumAsync(order => (decimal?)order.Price) ?? 0; // Nếu không có giá trị thì trả về 0

            return revenue;
        }

        public async Task<byte[]> ExportProductsToExcelAsync()
        {
            // Lấy danh sách sản phẩm từ database
            var products = await GetAllProductsAsync();

            // Sử dụng EPPlus để tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Tên sản phẩm";
                worksheet.Cells[1, 2].Value = "Giá";
                worksheet.Cells[1, 3].Value = "Số lượng";
                worksheet.Cells[1, 4].Value = "Ngày tạo";

                // Thêm dữ liệu sản phẩm
                int row = 2;
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Name;
                    worksheet.Cells[row, 2].Value = product.Price;
                    worksheet.Cells[row, 3].Value = product.Quantity;

                    // Đảm bảo định dạng ngày tháng cho Excel
                    worksheet.Cells[row, 4].Value = product.CreatedDate.HasValue
                        ? product.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : string.Empty; // Hoặc giá trị mặc định nào đó

                    row++;
                }

                // Tự động điều chỉnh kích thước cột
                worksheet.Cells.AutoFitColumns();

                // Lưu dữ liệu vào bộ nhớ và chuyển thành mảng byte
                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                    return stream.ToArray(); // Trả về mảng byte từ MemoryStream
                }
            }
        }
    }

}
