
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IMessage
    {
        Task<Messages> AddMessageWithConversation(string username, string userGive, Messages messageModel);
        Task<List<Messages>> GetMessagesByConversationIdAsync(int conversationId);
        Task<List<Messages>> GetMessagesBetweenUsers(string username, string selectedUser);
    }
}
