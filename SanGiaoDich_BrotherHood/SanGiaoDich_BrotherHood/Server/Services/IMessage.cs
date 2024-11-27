
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IMessage
    {
        Task<Conversation> CreateConversationAsync(string username1, string username2);
        Task<List<Conversation>> GetConversationsForUserAsync(string username);
        Task<List<Message>> GetMessagesByConversationIdAsync(int conversationId);
        Task<List<Message>> GetMessagesBetweenUsersAsync(string username1, string username2);
        Task<Message> SendMessageAsync(int conversationId, string userSend, string content, string typeContent);
    }
}
