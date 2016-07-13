using System.IO;
using System.Threading.Tasks;
using hMailServer.Entities;

namespace hMailServer.Repository
{
    public interface IMessageRepository
    {
        Task InsertAsync(Message message, Stream stream);
        Task<Message> GetMessageToDeliverAsync();
    }
}
