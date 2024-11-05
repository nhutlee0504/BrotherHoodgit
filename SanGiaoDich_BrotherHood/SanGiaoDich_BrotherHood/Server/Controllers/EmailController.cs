using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System;
using System.Text;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("BroderHood Support", "quangminh2045@gmail.com"));
                emailMessage.To.Add(new MailboxAddress("", emailRequest.To));
                emailMessage.Subject = emailRequest.Subject;
                emailMessage.Body = new TextPart("plain") { Text = emailRequest.Body };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, useSsl: false);
                    await client.AuthenticateAsync("quangminh2045@gmail.com", "ufut lmjy brif aeel");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
