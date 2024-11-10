

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

                // Kiểm tra xem tài khoản đã được tạo hay chưa
                if (acc != null)
                {
                    return Ok(acc); // Trả về thông tin tài khoản vừa tạo
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
        public async Task<IActionResult> GetAccountInfo()//Lấy thông tim bản thân
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
        public async Task<IActionResult> GetAccountByName(string username)//Xem thông tin tài khoản người khác
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
		public async Task<IActionResult> UpdateAccountInfo(InfoAccountDto infoAccountDto)
		{
			try
			{
				var updatedUser = await _user.UpdateAccountInfo(infoAccountDto);
				return Ok(updatedUser); // Return updated user info
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
        [Route("UpdateProfileImage")]
		public async Task<IActionResult> UpdateProfileImage(IFormFile imageFile)
		{
			try
			{
				var updatedUser = await _user.UpdateProfileImage(imageFile);
				return Ok(updatedUser); // Return updated user info
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

            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == email);

            string token;

            if (existingUser != null)
            {
                token = GenerateJwtToken(existingUser); // Tạo token cho người dùng đã có
            }
            else
            {
                var newUser = new Account
                {
                    UserName = email.Substring(0, 5),
                    Email = email,
                    Password = "default-password",
                    CreatedTime = DateTime.Now,
                    Role = "Người dùng",
                    IsDelete = false
                };

                _context.Accounts.Add(newUser);
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
            // Bạn có thể lưu token vào session hoặc thực hiện các xử lý khác
            // Ví dụ, trả về view hoặc làm gì đó với token
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token không hợp lệ.");
            }
            return Ok(token);
        }
        private string GenerateJwtToken(Account user)
        {
            // Kiểm tra người dùng không null
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Khóa bí mật để ký token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo danh sách claims chứa thông tin người dùng
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty), // Tên người dùng
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty), // Email
        new Claim(ClaimTypes.Role, user.Role ?? string.Empty), // Vai trò người dùng
        new Claim("FullName", user.FullName ?? string.Empty), // Họ và tên
        new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty), // Số điện thoại
        new Claim("Gender", user.Gender ?? string.Empty), // Giới tính
        new Claim("Birthday", user.Birthday?.ToString("o") ?? string.Empty), // Ngày sinh (chuyển sang định dạng chuẩn)
        new Claim("ImageAccount", user.ImageAccount ?? string.Empty), // Hình ảnh tài khoản
        new Claim("IsDelete", user.IsDelete.ToString()), // Trạng thái xóa
        new Claim("TimeBanned", user.TimeBanned?.ToString("o") ?? string.Empty) // Thời gian bị cấm (nếu có)
    };

            // Tạo JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"], // Issuer (người phát hành)
                audience: _configuration["JWT:ValidAudience"], // Audience (người nhận)
                claims: claims, // Claims chứa thông tin người dùng
                expires: DateTime.Now.AddMinutes(30), // Thời gian hết hạn (30 phút)
                signingCredentials: creds // Thông tin ký token
            );

            // Trả về token dưới dạng chuỗi
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("signin-facebook")]
        public IActionResult SignInFacebook()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse")  // Điều hướng sau khi Facebook xử lý đăng nhập
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

            var existingUser = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == email);

            string token;

            if (existingUser != null)
            {
                token = GenerateJwtToken(existingUser); // Tạo token cho người dùng đã có
            }
            else
            {
                var newUser = new Account
                {
                    UserName = email.Substring(0, 5),
                    Email = email,
                    Password = "default-password",
                    CreatedTime = DateTime.Now,
                    Role = "Người dùng",
                    IsDelete = false
                };

                _context.Accounts.Add(newUser);
                await _context.SaveChangesAsync();

                token = GenerateJwtToken(newUser);
            }

            // Trả về token và chuyển hướng về trang login kèm token trong URL
            return Redirect($"/login?token={token}");
        }


    }
}
