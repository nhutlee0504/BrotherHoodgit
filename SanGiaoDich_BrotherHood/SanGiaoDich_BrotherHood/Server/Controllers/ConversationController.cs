using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanGiaoDich_BrotherHood.Server.Services;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
       private readonly IConversation _conversation;
        public ConversationController(IConversation conversation) { _conversation = conversation; }
        [HttpPost("CreateConversation")]
        public async Task<IActionResult> Create(Conversation model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Conversation model is null");
                }

                var createdConversation = await _conversation.AddConversation(model);
                if (createdConversation == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error creating conversation");
                }

                return Ok(createdConversation);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
        [HttpGet("GetConversations/{username}")]
        public async Task<IActionResult> GetConversations(string username)
        {
            try
            {
                var conversations = await _conversation.GetConversationAsync(username);

                if (conversations == null || !conversations.Any())
                {
                    return NotFound("No conversations found.");
                }

                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
