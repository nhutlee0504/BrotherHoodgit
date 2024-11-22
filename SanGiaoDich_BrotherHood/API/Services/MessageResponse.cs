using API.Data;
using API.Dto;
using API.Models;
using Microsoft.AspNetCore.Http;
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

        // Gửi tin nhắn (Có thể là văn bản hoặc hình ảnh)
        public async Task<Message> SendMessage(SendMessage sendDto, IFormFile imageFile = null)
        {
            if (sendDto == null || string.IsNullOrEmpty(sendDto.UserSend) || sendDto.ConversationID == 0)
                throw new ArgumentException("Thông tin tin nhắn không hợp lệ");

            // Tạo đối tượng Message từ Dto
            var message = new Message
            {
                ConversationID = sendDto.ConversationID,
                UserSend = sendDto.UserSend,
                Content = sendDto.Content,
                TypeContent = string.IsNullOrEmpty(sendDto.Content) ? "Text" : "Text", // Mặc định là Text nếu không có nội dung khác
                CreatedDate = DateTime.Now.ToString(),
                Status = "Đã gửi", // Trạng thái ban đầu
                IsDeleted = false
            };

            // Nếu có tệp hình ảnh, lưu vào thư mục và thay đổi kiểu nội dung tin nhắn
            if (imageFile != null)
            {
                await SaveImage(imageFile);
                message.Content = "image"; // Nội dung tin nhắn là hình ảnh
                message.Status = "Đã gửi hình ảnh"; // Trạng thái tin nhắn
            }
            else
            {
                message.Status = "Đã gửi tin nhắn"; // Trạng thái tin nhắn nếu là văn bản
            }

            // Lưu tin nhắn vào cơ sở dữ liệu
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        // Lấy tin nhắn trong cuộc trò chuyện giữa người gửi và người nhận
        public async Task<IEnumerable<Message>> GetMessages(string userSend, string userRevice)
        {
            if (string.IsNullOrEmpty(userSend) || string.IsNullOrEmpty(userRevice))
                throw new ArgumentException("Thông tin người gửi và người nhận không hợp lệ");

            var conversation = await _context.Conversations
                .Where(c => (c.Username == userSend || c.Username == userRevice) && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (conversation == null)
                throw new Exception("Không tìm thấy cuộc trò chuyện giữa người gửi và người nhận");

            return await _context.Messages
                .Where(m => m.ConversationID == conversation.ConversationID && !m.IsDeleted)
                .OrderBy(m => m.CreatedDate)
                .ToListAsync();
        }

        // Tạo cuộc trò chuyện mới
        public async Task<Conversation> CreateChat(Conversation conversation)
        {
            var newCon = new Conversation
            {
                Username = conversation.Username,
                CreatedDate = DateTime.Now.ToString(),
                IsDeleted = false,
            };
            _context.Conversations.Add(newCon);
            await _context.SaveChangesAsync();
            return newCon;
        }

        // Helper method để lưu ảnh vào thư mục
        private async Task SaveImage(IFormFile imageFile)
        {
            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(_imagePath, fileName);

            // Kiểm tra thư mục AnhNhanTin, nếu chưa có thì tạo
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
        }
    }
}
