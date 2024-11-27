using API.Dto;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation([FromQuery] string username1, [FromQuery] string username2)
        {
            try
            {
                var conversation = await _messageService.CreateConversationAsync(username1, username2);
                return Ok(conversation); // Trả về đối tượng cuộc trò chuyện trực tiếp
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Chỉ trả về thông báo lỗi
            }
        }

        [HttpGet("get-conversations/{username}")]
        public async Task<IActionResult> GetConversationsForUser(string username)
        {
            try
            {
                // Giả sử phương thức GetConversationsForUserAsync trả về danh sách các cuộc trò chuyện
                var conversations = await _messageService.GetConversationsForUserAsync(username);

                // Lọc ra danh sách người tham gia cuộc trò chuyện
                var participants = conversations
                    .SelectMany(conversation => conversation.conversationParticipants)
                    .Where(participant => participant.UserName != null)  // Bạn có thể tùy chỉnh điều kiện lọc ở đây
                    .ToList();

                return Ok(participants); // Trả về danh sách chỉ bao gồm người tham gia cuộc trò chuyện
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("messages-by-conversation/{conversationId}")]
        public async Task<IActionResult> GetMessagesByConversationId(int conversationId)
        {
            try
            {
                var messages = await _messageService.GetMessagesByConversationIdAsync(conversationId);
                return Ok(messages); // Trả về danh sách tin nhắn trực tiếp
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Trả về thông báo lỗi
            }
        }

        [HttpGet("messages-between")]
        public async Task<IActionResult> GetMessagesBetweenUsers([FromQuery] string username1, [FromQuery] string username2)
        {
            try
            {
                var messages = await _messageService.GetMessagesBetweenUsersAsync(username1, username2);
                return Ok(messages); // Trả về danh sách tin nhắn trực tiếp
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Trả về thông báo lỗi
            }
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
        {
            try
            {
                var message = await _messageService.SendMessageAsync(
                    messageDto.ConversationId,
                    messageDto.UserSend,
                    messageDto.Content,
                    messageDto.TypeContent
                );

                return Ok(message); // Trả về tin nhắn trực tiếp
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Trả về thông báo lỗi
            }
        }

    }
}
