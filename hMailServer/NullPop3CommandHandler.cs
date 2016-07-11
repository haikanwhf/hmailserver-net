using System.Threading.Tasks;
using hMailServer.Core.Protocols.POP3;

namespace hMailServer
{
    public class NullPop3CommandHandler : IPop3ServerCommandHandler
    {
        public Task<Pop3CommandResult> HandleQuit()
        {
            return Task.Run(() => new Pop3CommandResult(true, "Goodbye"));
        }

        public Task<Pop3CommandResult> HandleStat()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleList()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleRetr()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleDele()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleNoop()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleRset()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleTop()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleUidl()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleUser()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandlePass()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleCapa()
        {
            throw new System.NotImplementedException();
        }

        public Task<Pop3CommandResult> HandleStls()
        {
            throw new System.NotImplementedException();
        }
    }
}