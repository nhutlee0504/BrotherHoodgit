using API.Data;
using API.Dto;
using API.Models;
using FirebaseAdmin.Auth.Hash;
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
                Birthday = DateTime.Now,
                Email = "admin@gmail.com",
                FullName = "Admin",
                PhoneNumber = "0987654321",
                Gender = "Nam",
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
            var userInfo = await _context.Accounts
            .FirstOrDefaultAsync(u => EF.Functions.Collate(u.UserName, "Latin1_General_BIN") == loginDto.UserName);
            if (userInfo == null || !VerifyPassword(loginDto.Password, userInfo.Password))
            {
                throw new UnauthorizedAccessException("Tên tài khoản hoặc mật khẩu không đúng.");
            }
            // Kiểm tra nếu tài khoản đã bị xóa
            if (userInfo.IsDelete == true)
            {
                throw new UnauthorizedAccessException("Tài khoản này đã bị khóa vô thời hạn.");
            }

            // Kiểm tra nếu tài khoản bị cấm
            if (userInfo.TimeBanned.HasValue)
            {
                if (userInfo.TimeBanned > DateTime.UtcNow)
                {
                    var remainingDays = (userInfo.TimeBanned.Value - DateTime.UtcNow).TotalDays;
                    throw new UnauthorizedAccessException($"Tài khoản này đã bị khóa. Số ngày còn lại: {Math.Ceiling(remainingDays)}.");
                }
                else
                {
                    // Nếu thời gian cấm đã hết, đặt TimeBanned thành null
                    userInfo.TimeBanned = null;
                    await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                }
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
            if (user.Role == "Chủ" || user.Role == "Nhân viên")
            {
                var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
                if (userFind != null)
                {
                    if (userFind.UserName != user.UserName)
                    {
                        if (userFind.Role != "Chủ" && userFind.Role != "Nhân viên")
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
            if (user.Role == "Nhân viên" && user.Role == "Chủ")
            {
                var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
                if (userFind != null)
                {
                    if (userFind.UserName != user.UserName)
                    {
                        if (userFind.Role != "Chủ" && userFind.Role != "Nhân viên")
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
            // Kiểm tra người dùng không null
            if (user == null) throw new ArgumentNullException(nameof(user));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
        new Claim("FullName", user.FullName ?? string.Empty),
        new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty),
        new Claim("Gender", user.Gender ?? string.Empty),
        new Claim("Birthday", user.Birthday?.ToString("o") ?? string.Empty),
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

        public async Task<Account> RegisterAccount(RegisterDto registerDto)
        {
            // Kiểm tra mật khẩu hợp lệ
            if (!IsValidPassword(registerDto.Password))
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.");
            }

            // Kiểm tra xem tên người dùng đã tồn tại chưa
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == registerDto.UserName);
            if (existingUser != null)
            {
                throw new ArgumentException("Tên người dùng đã tồn tại.");
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu có khớp không
            if (registerDto.Password != registerDto.ConformPassword)
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            // Gán quyền mặc định là "Nhân viên" nếu không có quyền trong request
            var role = string.IsNullOrEmpty(registerDto.Role) ? "Nhân viên" : registerDto.Role;

            // Kiểm tra vai trò hợp lệ (Chỉ cho phép 'Admin' hoặc 'Nhân viên')
            var validRoles = new List<string> { "Admin", "Nhân viên" };
            if (!validRoles.Contains(role))
            {
                throw new ArgumentException("Vai trò không hợp lệ. Vai trò hợp lệ là 'Admin' hoặc 'Nhân viên'.");
            }

            // Tạo tài khoản người dùng mới
            var newAccount = new Account
            {
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password), // Mã hóa mật khẩu
                IsDelete = false,
                CreatedTime = DateTime.Now,
                Role = role, // Phân quyền theo lựa chọn của người dùng hoặc mặc định "Nhân viên"
                PreSystem = 10000,  // Điểm hoặc thông số ban đầu cho người dùng
                IsActived = true   // Tài khoản mặc định được kích hoạt
            };

            // Thêm tài khoản mới vào cơ sở dữ liệu
            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync();

            return newAccount;
        }


    }
}
