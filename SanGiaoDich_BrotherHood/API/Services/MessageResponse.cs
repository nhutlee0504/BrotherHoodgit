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
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhNhanTin"); // Đường dẫn lưu hình ảnh
        }

        public async Task<Conversation> CreateConversationAsync(string username1, string username2)
        {
            // Kiểm tra xem đã tồn tại cuộc trò chuyện giữa hai người chưa
            var existingConversation = await _context.Conversations
                .Include(c => c.conversationParticipants)
                .FirstOrDefaultAsync(c =>
                    c.conversationParticipants.Any(cp => cp.UserName == username1) &&
                    c.conversationParticipants.Any(cp => cp.UserName == username2) &&
                    !c.IsDeleted);

            if (existingConversation != null)
            {
                return existingConversation; // Trả về nếu đã tồn tại
            }

            // Tạo cuộc trò chuyện mới
            var conversation = new Conversation
            {
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsDeleted = false
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            // Thêm hai người tham gia
            var participants = new List<ConversationParticipant>
    {
        new ConversationParticipant
        {
            ConversationId = conversation.ConversationID,
            UserName = username1,
            JoinedDate = DateTime.Now,
            IsDeleted = false
        },
        new ConversationParticipant
        {
            ConversationId = conversation.ConversationID,
            UserName = username2,
            JoinedDate = DateTime.Now,
            IsDeleted = false
        }
    };

            _context.ConversationParticipants.AddRange(participants);
            await _context.SaveChangesAsync();

            return conversation;
        }

        public async Task<List<Conversation>> GetConversationsForUserAsync(string username)
        {
            return await _context.Conversations
                .Where(c => c.conversationParticipants.Any(cp => cp.UserName == username) && !c.IsDeleted)
                .Include(c => c.conversationParticipants.Where(cp => cp.UserName != username)) // Lấy người dùng khác
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByConversationIdAsync(int conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationID == conversationId && !m.IsDeleted)
                .OrderBy(m => m.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesBetweenUsersAsync(string username1, string username2)
        {
            // Tìm cuộc trò chuyện giữa hai người
            var conversation = await _context.Conversations
                .Include(c => c.conversationParticipants)
                .FirstOrDefaultAsync(c =>
                    c.conversationParticipants.Any(cp => cp.UserName == username1) &&
                    c.conversationParticipants.Any(cp => cp.UserName == username2) &&
                    !c.IsDeleted);

            if (conversation == null)
            {
                return new List<Message>(); // Không có tin nhắn nếu không tìm thấy cuộc trò chuyện
            }

            return await GetMessagesByConversationIdAsync(conversation.ConversationID);
        }

        public async Task<Message> SendMessageAsync(int conversationId, string userSend, string content, string typeContent)
        {
            // Kiểm tra xem cuộc trò chuyện có tồn tại không
            var conversation = await _context.Conversations
                .Include(c => c.conversationParticipants)
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId && !c.IsDeleted);

            if (conversation == null)
            {
                throw new ArgumentException("Conversation not found.");
            }

            // Kiểm tra xem người gửi có thuộc cuộc trò chuyện không
            if (!conversation.conversationParticipants.Any(cp => cp.UserName == userSend))
            {
                throw new UnauthorizedAccessException("User is not part of this conversation.");
            }

            // Tạo tin nhắn mới
            var message = new Message
            {
                ConversationID = conversationId,
                UserSend = userSend,
                CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Content = content,
                Status = "Sent",
                IsDeleted = false,
                TypeContent = typeContent
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

    }
}
