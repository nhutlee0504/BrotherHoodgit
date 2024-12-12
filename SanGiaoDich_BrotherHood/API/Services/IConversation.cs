using API.Models;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IConversation
    {
        Task<Conversation> AddConversation(Conversation conversation);
        Task<Conversation> RemoveConversation(Conversation conversation);
    }
}
