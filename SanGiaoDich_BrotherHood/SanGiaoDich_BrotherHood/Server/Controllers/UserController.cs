

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

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;

        public UserController(IUser user)
        {
            _user = user;
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
    }
}
