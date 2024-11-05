
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public MessageResponse(ApplicationDbContext context)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AnhNhanTin");
        }

        public async Task<Message> SendMessage(Message message, IFormFile imageFile = null)
        {
            if (imageFile != null)
            {
                // Lưu hình ảnh vào thư mục
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(_imagePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Cập nhật thông tin hình ảnh vào message
                message.Image = fileName; // Lưu tên tệp hình ảnh
                message.TypeContent = "image"; // Đánh dấu kiểu nội dung là hình ảnh
            }
            else
            {
                message.TypeContent = "text"; // Đánh dấu kiểu nội dung là văn bản
            }

            message.sendingTime = DateTime.UtcNow; // Đặt thời gian gửi
            message.Status = "Đã gửi";
            _context.Messages.Add(message); // Thêm tin nhắn vào cơ sở dữ liệu
            await _context.SaveChangesAsync(); // Lưu thay đổi
            return message; // Trả về tin nhắn đã gửi
        }

        public async Task<IEnumerable<Message>> GetMessages(string usersend, string userrevice)
        {
            return await _context.Messages
                .Where(m => m.UserSend == usersend.ToString() && m.UserReceive == userrevice)
                .OrderBy(m => m.sendingTime)
                .ToListAsync();
        }
    }
}
