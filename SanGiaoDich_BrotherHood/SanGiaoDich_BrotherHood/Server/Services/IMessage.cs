
using Microsoft.AspNetCore.Http;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IMessage
    {
        public Task<Message> SendMessage(Message message, IFormFile imageFile);
        public Task<IEnumerable<Message>> GetMessages(string usersend, string userrevice);
    }
}
