using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using System.Threading.Tasks;
using System;
using SanGiaoDich_BrotherHood.Shared.Models;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessage _message;
        public MessageController(IMessage message)
        {
            _message = message;
        }
        [HttpPost("CreateMess")]
        public async Task<IActionResult> Create([FromQuery] string username, [FromQuery] string userGive, [FromBody] Messages model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(userGive))
                {
                    return BadRequest("Invalid data: Message model, username, or userGive is null or empty.");
                }

                var createdMessage = await _message.AddMessageWithConversation(username, userGive, model);

                if (createdMessage == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error creating message or conversation.");
                }

                return Ok(createdMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpGet("GetMessages/{conversationId}")]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            try
            {
                var messages = await _message.GetMessagesByConversationIdAsync(conversationId);
                if (messages == null || messages.Count == 0)
                {
                    return NotFound("No messages found for this conversation.");
                }

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetMessagesBetween")]
        public async Task<IActionResult> GetMessagesBetween(string username, string selectedUser)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(selectedUser))
                {
                    return BadRequest("Username hoặc SelectedUser không hợp lệ.");
                }

                var messages = await _message.GetMessagesBetweenUsers(username, selectedUser);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }

    }
}
