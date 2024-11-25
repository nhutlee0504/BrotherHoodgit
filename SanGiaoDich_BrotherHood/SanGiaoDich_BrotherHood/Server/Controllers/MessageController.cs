using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Dto;
using System.Linq;

namespace SanGiaoDich_BrotherHood.Server.Controllers
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
                var conversations = await _messageService.GetConversationsForUserAsync(username);

                // Kiểm tra và đảm bảo định dạng trả về là JSON
                Response.ContentType = "application/json";

                var participants = conversations
                    .SelectMany(conversation => conversation.conversationParticipants)
                    .Where(participant => participant.UserName != null)
                    .ToList();

                return Ok(participants);
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

        [HttpGet("messages-between/{username1}/{username2}")]
        public async Task<IActionResult> GetMessagesBetweenUsers([FromRoute] string username1, [FromRoute] string username2)
        {
            try
            {
                var messages = await _messageService.GetMessagesBetweenUsersAsync(username1, username2);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
