using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Threading.Tasks;
using System;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ESMSService _esmsService;

        public SmsController(ESMSService esmsService)
        {
            _esmsService = esmsService;
        }

        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
        {
            try
            {
                await _esmsService.SendSmsAsync(request.PhoneNumber, request.Message);
                return Ok("Tin nhắn đã được gửi thành công!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi gửi tin nhắn: " + ex.Message);
            }
        }
    }

    public class SmsRequest
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
