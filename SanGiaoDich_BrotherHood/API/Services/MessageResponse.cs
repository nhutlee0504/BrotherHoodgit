using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class MessageResponse : IMessage
    {
        private readonly ApplicationDbContext _context;
        private readonly string _imagePath;

        public MessageResponse(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhNhanTin"); 
        }

        public Task<Message> CreateMessage(int id)
        {
         
        }
    }
}
