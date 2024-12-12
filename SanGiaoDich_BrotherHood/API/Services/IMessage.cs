using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IMessage
    {
        Task<Message> AddMessageWithConversation(string username, string userGive, Message messageModel);
        Task<List<Message>> GetMessagesByConversationIdAsync(int conversationId);
        Task<List<Message>> GetMessagesBetweenUsers(string username, string selectedUser);
    }
}
