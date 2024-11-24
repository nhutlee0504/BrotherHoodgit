using SanGiaoDich_BrotherHood.Shared.Models;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IConversation
    {
        Task<ConversationRespone> AddConversation(ConversationRespone conversation);
        Task<ConversationRespone> DeleteConversation(ConversationRespone conversation);
    }
}
