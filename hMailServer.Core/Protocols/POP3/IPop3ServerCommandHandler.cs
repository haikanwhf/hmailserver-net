using System.IO;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.POP3
{
    public interface IPop3ServerCommandHandler
    {
        Task<Pop3CommandReply> HandleQuit();
        Task<Pop3CommandReply> HandleStat();
        Task<Pop3CommandReply> HandleList();
        Task<Stream> HandleRetr(int messageNumber);
        Task<Pop3CommandReply> HandleDele(int messageNumber);
        Task<Pop3CommandReply> HandleNoop();
        Task<Pop3CommandReply> HandleRset();
        Task<Pop3CommandReply> HandleTop();
        Task<Pop3CommandReply> HandleUidl();
        Task<Pop3CommandReply> HandleUser(string username);
        Task<Pop3CommandReply> HandlePass(string username, string password);
        Task<Pop3CommandReply> HandleCapa();
        Task<Pop3CommandReply> HandleStls();
    }
}
