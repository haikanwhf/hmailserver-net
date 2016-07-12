using System.Threading.Tasks;
using hMailServer.Core.Protocols;

namespace hMailServer.Core
{
    public interface ISession
    {
        Task HandleConnection(IConnection connection);

        Protocol Protocol { get; }
    }
}
