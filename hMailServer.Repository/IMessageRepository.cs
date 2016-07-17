using System.IO;
using System.Threading.Tasks;
using hMailServer.Entities;

namespace hMailServer.Repository
{
    public interface IMessageRepository
    {
        Task InsertAsync(Message message, Stream stream);
        Task UpdateAsync(Message message);
        Task DeleteAsync(Message message);

        Task<Message> GetMessageToDeliverAsync();
        Task<Message> CreateAccountLevelMessageAsync(Message message, Account account);
        
    }
}
