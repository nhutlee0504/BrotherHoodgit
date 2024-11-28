using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IConversation
    {
        Task<List<Conversation>> GetConversationAsync(string username);
        Task<Conversation> AddConversation(Conversation conversation);
        Task<Conversation> DeleteConversation(Conversation conversation);
    }
}
