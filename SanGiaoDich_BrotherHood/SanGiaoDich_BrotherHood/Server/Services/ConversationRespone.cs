using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class ConversationRespone : IConversation
    {
        private readonly ApplicationDbContext _context;
        public ConversationRespone(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Conversation> AddConversation(Conversation conversation)
        {
            try
            {
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
                return conversation;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xuất hiện lỗi khi thêm phiên nói chuyện: {ex.Message}");
                throw;
            }
        }

       

        public Task<Conversation> DeleteConversation(Conversation conversation)
        {
           throw new NotImplementedException();
        }

        public async Task<List<Conversation>> GetConversationAsync(string username)
        {
            return await _context.Conversations
               .Where(c => c.Username == username && !c.IsDeleted || c.UserGive == username)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }
    }
}
