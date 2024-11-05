using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IMessage
    {
        public Task<Message> SendMessage(Message message, IFormFile imageFile);
        public Task<IEnumerable<Message>> GetMessages(string usersend, string userrevice);
    }
}
