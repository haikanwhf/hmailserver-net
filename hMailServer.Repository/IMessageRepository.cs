using System.IO;
using System.Threading.Tasks;
using hMailServer.Entities;

namespace hMailServer.Repository
{
    public interface IMessageRepository
    {
        Task Insert(Message message, Stream stream);
    }
}
