using API.Dto;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Security.Principal;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _admin;
        private object newAccount;

        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }

        [HttpGet("GetAllAccount")]
        public async Task<IActionResult> GetAllAccount()//Lấy danh sách tài khoản
        {
            try
            {
                return Ok(await _admin.GetAllAccount());
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegisterDto registerDto)//đăng ký tài khoản Admin
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

        }

        [HttpPost("LoginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginDto userLoginDto)//Đăng nhập dành cho Admin
        {
            try
            {
                var token = await _admin.LoginAdmin(userLoginDto);
                //return Ok(new { Token = token, Message = "Đăng nhập thành công." });
                return Ok(token);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // Thông báo lỗi đơn giản
            }
        }

        [HttpPost("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(RegisterDto registerDto)
        {
            try
            {
                // Kiểm tra nếu Role là null hoặc rỗng và gán mặc định là "Nhân viên"
                var role = "Nhân viên";

                // Kiểm tra vai trò hợp lệ
                var validRoles = new List<string> { "Admin", "Nhân viên" };

                if (!validRoles.Contains(role))
                {
                    return BadRequest(new { Message = "Vai trò không hợp lệ. Vai trò hợp lệ là 'Admin' hoặc 'Nhân viên'." });
                }

                // Tạo tài khoản với quyền được chỉ định
                var account = await _admin.RegisterAccount(registerDto);

                // Gán quyền cho tài khoản (Nếu cần)
                account.Role = role;

                // Lưu thay đổi vào cơ sở dữ liệu nếu cần
                // await _dbContext.SaveChangesAsync();

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
