using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IMessage
    {
        public Task<Conversation> CreateChat(Conversation conversation);
        public Task<Message> SendMessage(SendMessage sendDto, IFormFile imageFile);
        public Task<IEnumerable<Message>> GetMessages(string usersend, string userrevice);
    }
}
