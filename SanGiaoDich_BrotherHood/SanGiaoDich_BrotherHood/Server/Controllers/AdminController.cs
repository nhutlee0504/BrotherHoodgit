using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Server.Dto;
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using System.Linq;
using System.Security.Principal;
using System.Collections.Generic;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _admin;
        private readonly ApplicationDbContext _context;
        private object newAccount;

        public AdminController(IAdmin admin, ApplicationDbContext context)
        {
            _admin = admin;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccount() // Lấy danh sách tài khoản
        {
            try
            {
                var accounts = await _context.Accounts.ToListAsync(); // Hoặc IQueryable<Account>

                if (accounts == null || !accounts.Any()) // Kiểm tra nếu không có phần tử nào
                {
                    return NotFound("Không có tài khoản nào.");
                }

                return Ok(accounts);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterDto registerDto) // Đăng ký tài khoản Admin
        {
            try
            {
                var acc = await _admin.RegisterAdmin(registerDto);
                return Ok(acc);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginDto userLoginDto) // Đăng nhập dành cho Admin
        {
            try
            {
                var token = await _admin.LoginAdmin(userLoginDto);
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message); // Thông báo lỗi đơn giản
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        [HttpPost("BanAccount")]
        public async Task<IActionResult> BanAccount(string username)
        {
            var ban = await _admin.BannedAccount(username);
            return Ok($"Tài khoản {username} đã bị cấm.");
        }

        [HttpPost("UnBanAccount")]
        public async Task<IActionResult> UnBanAccount(string username)
        {
            var ban = await _admin.UnBannedAccount(username);
            return Ok($"Tài khoản {username} đã mở khóa.");
        }
        [HttpPost]
        [Route("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(RegisterDto registerDto)
        {
            try
            {
                // Kiểm tra nếu không có quyền trong yêu cầu, gán quyền mặc định là "Nhân viên"
                var role = "Nhân viên";

                // Kiểm tra vai trò hợp lệ (Chỉ cho phép 'Admin' hoặc 'Nhân viên')
                var validRoles = new List<string> { "Admin", "Nhân viên" };
                if (!validRoles.Contains(role))
                {
                    return BadRequest(new { Message = "Vai trò không hợp lệ. Vai trò hợp lệ là 'Admin' hoặc 'Nhân viên'." });
                }

                // Đăng ký tài khoản với quyền đã chọn
                var account = await _admin.RegisterAccount(registerDto);  // Đăng ký tài khoản

                // Gán quyền cho tài khoản sau khi đăng ký
                account.Role = role;

                // Lưu thay đổi quyền vào cơ sở dữ liệu
                await _context.SaveChangesAsync();  // Ensure you're using _context here

                return Ok(new { Message = "Tạo tài khoản thành công", Account = account });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi hệ thống", Detail = ex.Message });
            }
        }

    }
}