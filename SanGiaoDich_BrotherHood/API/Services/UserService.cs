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
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Services
{
    public class UserService : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public IEnumerable<object> Accounts => throw new NotImplementedException();

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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
                UserName = registerDto.UserName,
                Password = HashPassword(registerDto.Password),
                IsDelete = false,
                CreatedTime = DateTime.Now,
                Role = "Người dùng",
                PreSystem = 10000,
                IsActived = true
            };
            await _context.Accounts.AddAsync(newAdmin);
            await _context.SaveChangesAsync();
            return newAdmin;
        }
        public async Task<string> LoginUser(LoginDto loginDto)
        {
            var userInfo = await _context.Accounts
        .FirstOrDefaultAsync(u => EF.Functions.Collate(u.UserName, "Latin1_General_BIN") == loginDto.UserName);
            if (userInfo == null || !VerifyPassword(loginDto.Password, userInfo.Password))
            {
                throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            if (userInfo.IsDelete == true)
            {
                throw new UnauthorizedAccessException("Tài khoản này đã bị khóa vô thời hạn.");
            }
            if (userInfo.TimeBanned.HasValue && userInfo.TimeBanned > DateTime.UtcNow)
            {
                var remainingDays = (userInfo.TimeBanned.Value - DateTime.UtcNow).TotalDays;
                throw new UnauthorizedAccessException($"Tài khoản này đã bị khóa. Số ngày còn lại: {Math.Ceiling(remainingDays)}.");
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
            //if (!IsValidPhone(infoAccountDto.Phone))
            //{
            //    throw new ArgumentException("Số điện thoại không hợp lệ");
            //}
            //user.FullName = infoAccountDto.FullName;
            user.Email = email;
            //user.PhoneNumber = infoAccountDto.Phone;
            //user.Gender = infoAccountDto.Gender;
            //user.Birthday = infoAccountDto.Birthday;
            //user.Introduce = infoAccountDto.Introduce;

            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<Account> UpdateProfileImage(IFormFile imageFile = null)
        {
            var userClaims = GetUserInfoFromClaims();
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userClaims.UserName);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy người dùng.");
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhAvatar");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var originalFileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, originalFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                user.ImageAccount = originalFileName;
            }

            await _context.SaveChangesAsync();
            return user;
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
                expires: DateTime.Now.AddMinutes(30),
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

        public Task<IEnumerable<Account>> GetAccounts()
        {
            throw new NotImplementedException();
        }
    }
}
