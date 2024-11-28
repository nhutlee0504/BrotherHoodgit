using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IMessage
    {
        public Task<Message> CreateMessage(int id);

    }
}
