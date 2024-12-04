
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Server.Dto;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdmin _admin;
        public AdminController(IAdmin admin)
        {
            _admin = admin;
        }
        [HttpGet]
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

        [HttpPost]
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
                return BadRequest(ex.Message);
            }
        }
    }
}
