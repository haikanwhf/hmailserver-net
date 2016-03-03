using System.IO;

namespace hMailServer.Core.Protocols.POP3
{
    public interface IPop3ServerCommandHandler
    {
        Pop3CommandResult HandleQuit();
        Pop3CommandResult HandleStat();
        Pop3CommandResult HandleList();
        Pop3CommandResult HandleRetr();
        Pop3CommandResult HandleDele();
        Pop3CommandResult HandleNoop();
        Pop3CommandResult HandleRset();
        Pop3CommandResult HandleTop();
        Pop3CommandResult HandleUidl();
        Pop3CommandResult HandleUser();
        Pop3CommandResult HandlePass();
        Pop3CommandResult HandleCapa();
        Pop3CommandResult HandleStls();
    }
}
