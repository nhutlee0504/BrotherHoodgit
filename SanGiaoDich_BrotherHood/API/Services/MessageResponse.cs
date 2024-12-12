//using API.Data;
//using API.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//namespace API.Services
//{
//    public class MessageResponse : IMessage
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly string _imagePath;

//        private readonly string _connectionString;

//        public MessageResponse(ApplicationDbContext context, IConfiguration configuration)
//        {
//            _context = context;
//            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhNhanTin");
//            _connectionString = configuration.GetConnectionString("DefaultConnection");
//        }
//        public async Task<Message> AddMessageWithConversation(string username, string userGive, Message messageModel)
//        {
//            try
//            {
//                var existingConversation = await _context.Conversations
//                    .FirstOrDefaultAsync(c =>
//                        (c.UserName == username && c.UserName == userGive ||
//                         c.UserName == userGive && c.UserName == username)
//                        && !c.IsDeleted);

//                if (existingConversation == null)
//                {
//                    var newConversation = new Conversation
//                    {
//                        UserName = username,
//                        CreatedDate = DateTime.Now,
//                        IsDeleted = false
//                    };

//                    _context.Conversations.Add(newConversation);
//                    await _context.SaveChangesAsync();
//                    messageModel.ConversationID = newConversation.ConversationID;
//                }
//                else
//                {
//                    messageModel.ConversationID = existingConversation.ConversationID;
//                }
//                _context.Messages.Add(messageModel);
//                await _context.SaveChangesAsync();

//                return messageModel;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi khi thêm tin nhắn: {ex.Message}");
//                throw;
//            }
//        }
//        public async Task<List<Message>> GetMessagesByConversationIdAsync(int conversationId)
//        {
//            return await _context.Messages
//                .Where(m => m.ConversationID == conversationId && !m.IsDeleted)
//                .OrderBy(m => m.CreatedDate) // Tin nhắn theo thứ tự thời gian
//                .ToListAsync();
//        }
//        public async Task<List<Message>> GetMessagesBetweenUsers(string username, string selectedUser)
//        {
//            try
//            {
//                // Lấy ConversationID của hội thoại giữa username và selectedUser
//                var conversation = await _context.Conversations
//                    .FirstOrDefaultAsync(c =>
//                        (c.UserName == username && c.UserName == selectedUser) ||
//                        (c.UserName == selectedUser && c.UserName == username));

//                // Nếu không tìm thấy hội thoại, trả về danh sách rỗng
//                if (conversation == null)
//                {
//                    return new List<Message>();
//                }
//                var messages = await _context.Messages
//                    .Where(m => m.ConversationID == conversation.ConversationID && !m.IsDeleted)
//                    .OrderBy(m => m.CreatedDate)
//                    .ToListAsync();

//                return messages;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Lỗi khi tải tin nhắn: {ex.Message}");
//                throw;
//            }
//        }
//    }
//}
