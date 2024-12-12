
using Dapper;
using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class MessageResponse : IMessage
    {
        private readonly ApplicationDbContext _context;
        private readonly string _imagePath;

        private readonly string _connectionString;

        public MessageResponse(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhNhanTin");
            _connectionString = configuration.GetConnectionString("DefaultConnection"); 
        }
        public async Task<Messages> AddMessageWithConversation(string username, string userGive, Messages messageModel)
        {
            try
            {
                var existingConversation = await _context.Conversations
                    .FirstOrDefaultAsync(c =>
                        (c.Username == username && c.UserGive == userGive ||
                         c.Username == userGive && c.UserGive == username)
                        && !c.IsDeleted);

                if (existingConversation == null)
                {
                    var newConversation = new Conversation
                    {
                        Username = username,
                        UserGive = userGive,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    _context.Conversations.Add(newConversation);
                    await _context.SaveChangesAsync();
                    messageModel.ConversationID = newConversation.ConversationID;
                }
                else
                {
                    messageModel.ConversationID = existingConversation.ConversationID;
                }
                _context.Messages.Add(messageModel);
                await _context.SaveChangesAsync();

                return messageModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm tin nhắn: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Messages>> GetMessagesByConversationIdAsync(int conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationID == conversationId && !m.IsDeleted)
                .OrderBy(m => m.CreatedDate) // Tin nhắn theo thứ tự thời gian
                .ToListAsync();
        }
        public async Task<List<Messages>> GetMessagesBetweenUsers(string username, string selectedUser)
        {
            try
            {
                // Lấy ConversationID của hội thoại giữa username và selectedUser
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c =>
                        (c.Username == username && c.UserGive == selectedUser) ||
                        (c.Username == selectedUser && c.UserGive == username));

                // Nếu không tìm thấy hội thoại, trả về danh sách rỗng
                if (conversation == null)
                {
                    return new List<Messages>();
                }
                var messages = await _context.Messages
                    .Where(m => m.ConversationID == conversation.ConversationID && !m.IsDeleted)  
                    .OrderBy(m => m.CreatedDate)  
                    .ToListAsync();  

                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải tin nhắn: {ex.Message}");
                throw;
            }
        }

    }
}
