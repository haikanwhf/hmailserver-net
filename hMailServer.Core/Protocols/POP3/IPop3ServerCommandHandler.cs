using System.IO;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.POP3
{
    public interface IPop3ServerCommandHandler
    {
        Task<Pop3CommandResult> HandleQuit();
        Task<Pop3CommandResult> HandleStat();
        Task<Pop3CommandResult> HandleList();
        Task<Pop3CommandResult> HandleRetr();
        Task<Pop3CommandResult> HandleDele();
        Task<Pop3CommandResult> HandleNoop();
        Task<Pop3CommandResult> HandleRset();
        Task<Pop3CommandResult> HandleTop();
        Task<Pop3CommandResult> HandleUidl();
        Task<Pop3CommandResult> HandleUser(string username);
        Task<Pop3CommandResult> HandlePass(string username, string password);
        Task<Pop3CommandResult> HandleCapa();
        Task<Pop3CommandResult> HandleStls();
    }
}
