using API.Data;
using API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace API.Services
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
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", conversation.Username); 
                    var result = await db.QuerySingleOrDefaultAsync<Conversation>(
                        "AddConversation", 
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        // Xóa một Conversation
        public async Task<Conversation> RemoveConversation(int id)
        {
            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
                return null;

            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

      
     

        public Task<Conversation> RemoveConversation(Conversation conversation)
        {
            throw new NotImplementedException();
        }
    }
}
