using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversation _conversationService;

        public ConversationController(IConversation conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("AddConver")]
        public async Task<ActionResult<Conversation>> AddConversation(Conversation conversation)
        {
            if (conversation == null)
                return BadRequest(new { message = "Conversation data is required." });

            var addedConversation = await _conversationService.AddConversation(conversation);
            if (addedConversation == null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new { message = "An error occurred while creating the conversation." });

            // Trả về phản hồi thành công với mã 201 Created và đối tượng mới tạo
            return Created(string.Empty, addedConversation);
        }


        //// API: PUT /api/conversation/{id}
        //[HttpPut("{id}")]
        //public async Task<ActionResult<Conversation>> UpdateConversation(int id, [FromBody] Conversation conversation)
        //{
        //    if (conversation == null || id != conversation.Id)
        //        return BadRequest("Invalid conversation data.");

        //    var updatedConversation = await _conversationService.UpdateConversation(id, conversation);
        //    if (updatedConversation == null)
        //        return NotFound();

        //    return Ok(updatedConversation);
        //}

        //// API: DELETE /api/conversation/{id}
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Conversation>> RemoveConversation(int id)
        //{
        //    var deletedConversation = await _conversationService.RemoveConversation(id);
        //    if (deletedConversation == null)
        //        return NotFound();

        //    return Ok(deletedConversation);
        //}
    }
}
