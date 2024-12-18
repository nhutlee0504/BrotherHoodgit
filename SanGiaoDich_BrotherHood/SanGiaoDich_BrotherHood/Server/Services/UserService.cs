
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class UserService : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly FirebaseStorageService _firebaseService;
        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, FirebaseStorageService firebaseService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _firebaseService = firebaseService;
        }
        public string HashFullName(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString.Substring(0, 10);
            }
        }
        public async Task<Account> RegisterUser(RegisterDto registerDto)
        {
            if (!IsValidPassword(registerDto.Password))
            {
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt");
            }

            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == registerDto.UserName);
            if (existingUser != null)
            {
                throw new ArgumentException("Tên người dùng đã tồn tại.");
            }
            if (registerDto.Password != registerDto.ConformPassword)
            {
                throw new ArgumentException("Mật khẩu và xác nhận mật khẩu không khớp.");
            }
            var newAdmin = new Account
            {
                FullName = HashFullName(registerDto.UserName),
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password),
                IsDelete = false,
                CreatedTime = DateTime.Now,
                Role = "Người dùng",
                PreSystem = 25000,
                IsActived = true,
                ImageAccount = "https://firebasestorage.googleapis.com/v0/b/dbbrotherhood-ac2f1.appspot.com/o/ImageTest%2Favatar-default.svg?alt=media&token=fb4e7099-b322-412e-9da7-0f80d2311785"
            };
            var newCart = new Cart
            {
                UserName = newAdmin.UserName,
            };
            
            await _context.Accounts.AddAsync(newAdmin);
            await _context.Carts.AddAsync(newCart);
            await _context.SaveChangesAsync();
            return newAdmin;
        }
        public async Task<string> LoginUser(LoginDto loginDto)
        {
			var userInfo = await _context.Accounts
		.FirstOrDefaultAsync(u => EF.Functions.Collate(u.UserName, "Latin1_General_BIN") == loginDto.UserName);
			if (userInfo == null || !VerifyPassword(loginDto.Password, userInfo.Password))
            {
                throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");
            }
            if (userInfo.IsDelete == true)
            {
                throw new UnauthorizedAccessException("Tài khoản này đã bị khóa vô thời hạn");
            }
            if (userInfo.TimeBanned.HasValue && userInfo.TimeBanned > DateTime.UtcNow)
            {
                var remainingDays = (userInfo.TimeBanned.Value - DateTime.UtcNow).TotalDays;
                throw new UnauthorizedAccessException($"Tài khoản này đã bị khóa. Số ngày còn lại: {Math.Ceiling(remainingDays)}.");
            }
            if(userInfo.Role == "Chủ" || userInfo.Role == "Nhân viên")
            {
				throw new UnauthorizedAccessException("Không có quyền truy cập");
			}
            var token = GenerateJwtToken(userInfo);

            return token;
        }
        public async Task<Account> GetAccountInfo()//Lấy thông tin tài khoản đã đăng nhập vào hệ thống, bản thân
        {
            var userClaims = GetUserInfoFromClaims();

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
            }

            return user;
        }
        public async Task<Account> GetAccountByUserName(string userName)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new NotImplementedException("Không tìm thấy người dùng.");
            }

            return user; 
        }
        public async Task<Account> UpdateAccountInfo(string email)
        {
            var userClaims = GetUserInfoFromClaims();
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy người dùng.");
            }
            user.Email = email;

            await _context.SaveChangesAsync();
            return user; 
        }
        public async Task<Account> UpdateAccountInfo2(string phone, string description)
        {
            var userClaims = GetUserInfoFromClaims();
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy người dùng.");
            }
            user.PhoneNumber = phone;
            user.Introduce = description;

            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<Account> UpdateProfileImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("File ảnh không hợp lệ.");
            }

            var userClaims = GetUserInfoFromClaims();
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);
            if (user == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại.");
            }
            if (user.UserName != userClaims.UserName)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền cập nhật ảnh cho tài khoản này.");
            }

            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

                using (var stream = imageFile.OpenReadStream())
                {
                    var downloadUrl = await _firebaseService.UploadFileToFirebaseStorage(stream, uniqueFileName);
                    user.ImageAccount = downloadUrl; 
                    _context.Accounts.Update(user);
                    await _context.SaveChangesAsync();
                }

                return user; 
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải ảnh lên Firebase: {ex.Message}");
            }
        }

        public async Task<Account> ChangePassword(string username, InfoAccountDto info)
        {
            if (!IsValidPassword(info.Password))
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt");
            var userFind = await _context.Accounts.FirstOrDefaultAsync(x => x.UserName == username);
            userFind.Password = HashPassword(info.Password);
            await _context.SaveChangesAsync();
            return userFind;
        }

        public async Task<IEnumerable<Account>> GetAllAccount()
        {
            var get = await _context.Accounts.ToListAsync();
            if (get == null)
                throw new NotImplementedException("Không tìm thấy danh sách");
            return get;
        }

        private string GenerateJwtToken(Account user)
        {
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
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
              
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

            
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString(); 
            }
        }
        private bool VerifyPassword(string password, string hashedPasswordWithSalt) 
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

         
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                var hashedPassword = sb.ToString();

         
                return hashedPasswordWithSalt == hashedPassword; 
            }
        }
        private bool IsValidPassword(string password)
        {
            return password.Length >= 6 && 
                   password.Any(char.IsUpper) && 
                   password.Any(char.IsLower) && 
                   password.Any(char.IsDigit) && 
                   password.Any(ch => "!@#$%^&*()_-+=<>?/[]{}|~".Contains(ch));
        }
        public bool IsValidPhone(string phone)//Bắt lỗi số điện thoại
        {
            string pattern = @"^(?:\+84|0)(?:3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9]|2[0-9]{1})\d{7}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phone);
        }
        public async Task<Dictionary<string, int>> GetUserStatisticsAsync()
        {
            try
            {
                // Đếm tổng số người dùng
                int totalUsers = await _context.Accounts.CountAsync();

                // Đếm số người dùng hoạt động
                int activeUsers = await _context.Accounts.CountAsync(a =>
                        (a.IsDelete == false || a.IsDelete == null) && a.IsActived == true);

                // Số người dùng ngừng hoạt động
                int inactiveUsers = totalUsers - activeUsers;

                return new Dictionary<string, int>
                {
                    { "TotalUsers", totalUsers },
                    { "ActiveUsers", activeUsers },
                    { "InactiveUsers", inactiveUsers }
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê người dùng: {ex.Message}");
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

        public async Task<Account> AcceptIDCard(RecognitionDto recognitionDto)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserName == recognitionDto.UserName);

            if (account == null)
            {
                throw new ArgumentException("Không tìm thấy tài khoản với username đã cung cấp.");
            }
            account.FullName = recognitionDto.Name;
            account.Gender = recognitionDto.Sex;
            account.ID = recognitionDto.Id;
            account.Dob = recognitionDto.Dob;
            account.Home = recognitionDto.Home;
            account.Nationality = recognitionDto.Nationality;
            account.Doe = recognitionDto.Doe;
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<MemoryStream> ExportUserStatisticsToExcelAsync()
        {
            var statistics = await GetUserStatisticsAsync(); // Gọi phương thức lấy thống kê người dùng

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("User Statistics");

                // Tiêu đề cột
                worksheet.Cells[1, 1].Value = "Loại Người Dùng";
                worksheet.Cells[1, 2].Value = "Số Lượng";

                // Dữ liệu thống kê
                worksheet.Cells[2, 1].Value = "Tổng Số Người Dùng";
                worksheet.Cells[2, 2].Value = statistics["TotalUsers"];

                worksheet.Cells[3, 1].Value = "Người Dùng Hoạt Động";
                worksheet.Cells[3, 2].Value = statistics["ActiveUsers"];

                worksheet.Cells[4, 1].Value = "Người Dùng Ngừng Hoạt Động";
                worksheet.Cells[4, 2].Value = statistics["InactiveUsers"];

                // Đảm bảo các ô có định dạng
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Trả về file Excel dưới dạng MemoryStream
                var memoryStream = new MemoryStream();
                await package.SaveAsAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
