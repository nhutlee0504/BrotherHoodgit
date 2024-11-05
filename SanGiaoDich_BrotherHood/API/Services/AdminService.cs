using API.Data;
using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public class AdminService : IAdmin
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; // Thêm IConfiguration
        public AdminService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<Account> RegisterAdmin(RegisterDto registerDto)//Tạo tài khoản Admin
        {
            // Kiểm tra quy chuẩn mật khẩu
            if (!IsValidPassword(registerDto.Password))
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.");
            }

            // Kiểm tra xem username đã tồn tại hay chưa
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == registerDto.UserName);
            if (existingUser != null)
            {
                throw new ArgumentException("Tên người dùng đã tồn tại.");
            }
            // Kiểm tra nếu mật khẩu và xác nhận mật khẩu không khớp
            if (registerDto.Password != registerDto.ConformPassword)
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không khớp.");
            }
            var newAdmin = new Account
            {
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password),
                IsDelete = false,
                CreatedTime = DateTime.Now,
                Role = "Chủ"
            };
            await _context.Accounts.AddAsync(newAdmin);
            await _context.SaveChangesAsync();
            return newAdmin;
        }
        public async Task<string> LoginAdmin(LoginDto loginDto)//Đăng nhập dành cho Admin
        {
            var userInfo = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (userInfo == null || !VerifyPassword(loginDto.Password, userInfo.Password))
            {
                throw new UnauthorizedAccessException("Tên tài khoản hoặc mật khẩu không đúng.");
            }
            // Kiểm tra vai trò
            if (userInfo.Role == "Chủ" || userInfo.Role == "Nhân viên")
            {
                var token = GenerateJwtToken(userInfo);
                return token;
            }
            throw new UnauthorizedAccessException("Bạn không có quyền truy cập vào tài khoản Admin.");

        }
        public async Task<IEnumerable<Account>> GetAllAccount()//Lây tất cả tài khoản
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role != "Chủ")
            {
                throw new UnauthorizedAccessException("Chỉ admin mới có quyền");
            }

            var users = await _context.Accounts.ToListAsync();
            if (users == null)
            {
                throw new NotImplementedException("Không tìm thấy người dùng nào");
            }
            return users;
        }
        public async Task<Account> BannedAccountHigh(string username, DateTime endDate)//Ban tài khoản theo thời hạn
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Chủ")
            {
                var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
                if (userFind != null)
                {
                    if (userFind.UserName != user.UserName)
                    {
                        if (userFind.Role != "Chủ")
                        {
                            userFind.TimeBanned = endDate;
                            await _context.SaveChangesAsync();
                            return userFind;
                        }
                        throw new UnauthorizedAccessException("Bạn không thể khóa tài khoản quyền cao hơn");
                    }
                    throw new UnauthorizedAccessException("Bạn không thể khóa chính mình");
                }
                throw new System.NotImplementedException("Không tìm thấy tài khoản");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng");
        }
        public async Task<Account> BannedAccountLow(string username, DateTime endDate)//Ban tài khoản theo thời hạn
        {
            var user = GetUserInfoFromClaims();
            if(user.Role == "Chủ" || user.Role == "Nhân viên")
            {
                var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
                if (userFind != null)
                {
                    if(userFind.UserName != user.UserName)
                    {
                        if(userFind.Role != "Chủ" && userFind.Role != "Nhân viên")
                        {
                            userFind.TimeBanned = endDate;
                            await _context.SaveChangesAsync();
                            return userFind;
                        }
                        throw new UnauthorizedAccessException("Bạn không thể khóa tài khoản quyền cao hơn");
                    }
                    throw new UnauthorizedAccessException("Bạn không thể khóa chính mình");
                }
                throw new System.NotImplementedException("Không tìm thấy tài khoản");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng");
        }
        public async Task<Account> DeleteAccountHigh(string username)//Khóa tài khoản vô thời hạn
        {
            var user = GetUserInfoFromClaims();
            if (user.Role == "Chủ")
            {
                var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
                if (userFind != null)
                {
                    if (userFind.UserName != user.UserName)
                    {
                        if (userFind.Role != "Chủ")
                        {
                            userFind.IsDelete = true;
                            await _context.SaveChangesAsync();
                            return userFind;
                        }
                        throw new UnauthorizedAccessException("Không thể xóa tài khoản có quyền cao hơn");
                    }
                    throw new UnauthorizedAccessException("Bạn không thể xóa chính mình");
                }
                throw new NotImplementedException("Không tìm thấy tài khoản");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này");
        }
        public async Task<Account> DeleteAccountLow(string username)//Khóa tài khoản vô thời hạn
        {
            var user = GetUserInfoFromClaims();
            if(user.Role == "Nhân viên" && user.Role == "Chủ")
            {
                var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
                if (userFind != null)
                {
                    if(userFind.UserName != user.UserName)
                    {
                        if(userFind.Role != "Chủ" && userFind.Role != "Nhân viên")
                        {
                            userFind.IsDelete = true;
                            await _context.SaveChangesAsync();
                            return userFind;
                        }
                        throw new UnauthorizedAccessException("Không thể xóa tài khoản có quyền cao hơn");
                    }
                    throw new UnauthorizedAccessException("Bạn không thể xóa chính mình");
                }
                throw new NotImplementedException("Không tìm thấy tài khoản");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng này");
        }
        public async Task<Product> CensorProduct(int idProd, string sensor)
        {
            var user = GetUserInfoFromClaims();
            var productFind = await _context.Products.FirstOrDefaultAsync(x => x.IDProduct == idProd);
            if (productFind != null)
            {
                productFind.Status = sensor;
                await _context.SaveChangesAsync();
                return productFind;
            }
            throw new System.NotImplementedException("Không tìm thấy sản phẩm");
        }
        public Task<Rating> CensorRating(int idRating)
        {
            throw new System.NotImplementedException();
        }

        //Phương thức ngoài
        private string GenerateJwtToken(Account user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("FullName", user.FullName),
        new Claim("PhoneNumber", user.PhoneNumber),
        new Claim("Gender", user.Gender),
        new Claim("Birthday", user.Birthday?.ToString("o")),
        new Claim("ImageAccount", user.ImageAccount ?? string.Empty),
        new Claim("IsDelete", user.IsDelete.ToString()),
        new Claim("TimeBanned", user.TimeBanned?.ToString("o") ?? string.Empty)
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password) // Băm mật khẩu
        {
            using (var sha256 = SHA256.Create())
            {
                // Băm mật khẩu
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Chỉ lấy 16 byte đầu tiên và chuyển đổi sang định dạng chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString(); // Trả về mật khẩu băm
            }
        }

        private bool VerifyPassword(string password, string hashedPasswordWithSalt) // Kiểm tra mật khẩu khi đăng nhập
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Chỉ lấy 16 byte đầu tiên và chuyển đổi sang định dạng chuỗi hex
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                var hashedPassword = sb.ToString();

                // So sánh mật khẩu đã băm với mật khẩu đã lưu trong cơ sở dữ liệu
                return hashedPasswordWithSalt == hashedPassword; // So sánh trực tiếp
            }
        }
        private bool IsValidPassword(string password)//Bắt lỗi quy chuẩn password
        {
            return password.Length >= 6 && // Độ dài tối thiểu
                   password.Any(char.IsUpper) && // Có ít nhất một chữ hoa
                   password.Any(char.IsLower) && // Có ít nhất một chữ thường
                   password.Any(char.IsDigit) && // Có ít nhất một số
                   password.Any(ch => "!@#$%^&*()_-+=<>?/[]{}|~".Contains(ch)); // Có ít nhất một ký tự đặc biệt
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
