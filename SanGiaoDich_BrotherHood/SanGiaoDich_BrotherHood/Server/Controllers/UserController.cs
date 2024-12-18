

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Server.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Client.Pages;
using System.Security.Claims;
using SanGiaoDich_BrotherHood.Server.Data;
using System.Linq;
using SanGiaoDich_BrotherHood.Shared.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Facebook;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(IUser user, ApplicationDbContext context, IConfiguration configuration)
        {
            _user = user;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
        {
            try
            {
                var acc = await _user.RegisterUser(registerDto);
                if (acc != null)
                {
                    return Ok(acc);
                }

                return BadRequest("Đăng ký không thành công.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _user.LoginUser(loginDto);

                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("GetMyInfo")]
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
                var acc = await _user.GetAccountInfo();
                return Ok(acc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("GetAccountInfoByName/{username}")]
        public async Task<IActionResult> GetAccountByName(string username)
        {
            try
            {
                var acc = await _user.GetAccountByUserName(username);
                return Ok(acc);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

		[HttpPut]
        [Route("UpdateAccountInfo")]
		public async Task<IActionResult> UpdateAccountInfo(string email)
		{
			try
			{
				var updatedUser = await _user.UpdateAccountInfo(email);
				return Ok(updatedUser);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

        [HttpPut]
        [Route("UpdateAccountInfo2")]
        public async Task<IActionResult> UpdateAccountInfo2(string phone, string description)
        {
            try
            {
                var updatedUser = await _user.UpdateAccountInfo2(phone, description);
                return Ok(updatedUser);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("UpdateProfileImage")]
		public async Task<IActionResult> UpdateProfileImage(IFormFile imageFile)
		{
			try
			{
				var updatedUser = await _user.UpdateProfileImage(imageFile);
				return Ok(updatedUser);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPut]
        [Route("ChangePassword/{username}")]
        public async Task<IActionResult> ChangePassword(string username, InfoAccountDto info)
        {
            try
            {
                return Ok(await _user.ChangePassword(username, info));
            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAllAccount()
        {
            try
            {
                return Ok(await _user.GetAllAccount());
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("signin-google")]
        public IActionResult SignInGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")  // Điều hướng sau khi Google xử lý đăng nhập
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction(nameof(LogginSai));
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Không thể lấy email từ tài khoản Google." });
            }

            var userName = email.Contains("@") ? email.Split('@')[0] : email;
            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userName);

            string token;

            if (existingUser != null)
            {
                token = GenerateJwtToken(existingUser); // Tạo token cho người dùng đã có
            }
            else
            {
                var newUser = new Account
                {
                    UserName = userName,
                    Email = email,
                    Password = "default-password",
                    CreatedTime = DateTime.Now,
                    Role = "Người dùng",
                    IsDelete = false,
                    PreSystem = 10000,
                    IsActived = true
                };
                var newCart = new Cart
                {
                    UserName = newUser.UserName
                };
                _context.Accounts.Add(newUser);
                _context.Carts.Add(newCart);    
                await _context.SaveChangesAsync();

                token = GenerateJwtToken(newUser);
            }

            // Trả về token và chuyển hướng về trang login kèm token trong URL
            return Redirect($"/login?token={token}");
        }



        [HttpGet("LogginSai")]
        public async Task<IActionResult> LogginSai()
        {
            return BadRequest("Đăng nhập không thành công");
        }

        [HttpGet("LogginDung")]
        public IActionResult LogginDung(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token không hợp lệ.");
            }
            return Ok(token);
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
                signingCredentials: creds 
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("signin-facebook")]
        public IActionResult SignInFacebook()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse")  
            };

            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction(nameof(LogginSai));
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Không thể lấy email từ tài khoản Facebook." });
            }

            var userName = email.Contains("@") ? email.Split('@')[0] : email;

            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.UserName == userName);

            string token;

            if (existingUser != null)
            {
                token = GenerateJwtToken(existingUser); // Tạo token cho người dùng đã có
            }
            else
            {
                var newUser = new Account
                {
                    UserName = userName,
                    Email = email,
                    Password = "default-password",
                    CreatedTime = DateTime.Now,
                    Role = "Người dùng",
                    IsDelete = false,
                    PreSystem = 25000,
                    IsActived = true,
                };

                _context.Accounts.Add(newUser);
                await _context.SaveChangesAsync();

                token = GenerateJwtToken(newUser);
            }

         
            return Redirect($"/login?token={token}");
        }
        [HttpPost("AcceptIDCard")]
        public async Task<IActionResult> AcceptIDCard([FromBody] RecognitionDto recognitionDto)
        {
            try
            {
                var account = await _user.AcceptIDCard(recognitionDto);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("UserStatistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                // Gọi phương thức thống kê từ service
                var statistics = await _user.GetUserStatisticsAsync();

                // Trả về kết quả thống kê
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Lỗi khi lấy thống kê người dùng: {ex.Message}" });
            }
        }

        [HttpGet("ExportUserStatisticsExcel")]
        public async Task<IActionResult> ExportUserStatisticsExcel()
        {
            try
            {
                var excelData = await _user.ExportUserStatisticsToExcelAsync();
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserStatistics.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi xuất file Excel: " + ex.Message });
            }
        }

    }
}
