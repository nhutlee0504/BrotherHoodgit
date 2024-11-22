using API.Dto;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessage _messageService;

        public MessageController(IMessage messageService)
        {
            _messageService = messageService;
        }

        // Gửi tin nhắn (Có thể là văn bản hoặc hình ảnh)
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm] SendMessage sendDto, IFormFile imageFile = null)
        {
            if (sendDto == null)
                return BadRequest("Thông tin tin nhắn không hợp lệ");

            try
            {
                // Gửi tin nhắn và xử lý hình ảnh nếu có
                var message = await _messageService.SendMessage(sendDto, imageFile);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getmessages")]
        public async Task<IActionResult> GetMessages(string userSend, string userRevice)
        {
            if (string.IsNullOrEmpty(userSend) || string.IsNullOrEmpty(userRevice))
                return BadRequest("Thông tin người gửi và người nhận không hợp lệ");

            try
            {
                // Lấy danh sách tin nhắn giữa người gửi và người nhận
                var messages = await _messageService.GetMessages(userSend, userRevice);

                // Tạo JsonSerializerOptions với ReferenceHandler.Preserve để xử lý vòng lặp tham chiếu
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true // Tùy chọn này giúp JSON dễ đọc hơn trong quá trình phát triển
                };

                // Trả về danh sách tin nhắn với tùy chọn serializer đã thiết lập
                var jsonResult = JsonSerializer.Serialize(messages, options);
                return Ok(jsonResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Tạo cuộc trò chuyện mới
        [HttpPost("createchat")]
        public async Task<IActionResult> CreateChat([FromBody] Conversation conversation)
        {
            if (conversation == null)
                return BadRequest("Thông tin cuộc trò chuyện không hợp lệ");

            try
            {
                // Tạo cuộc trò chuyện mới
                var newConversation = await _messageService.CreateChat(conversation);
                return CreatedAtAction(nameof(CreateChat), new { id = newConversation.ConversationID }, newConversation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
