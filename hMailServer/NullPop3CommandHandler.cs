using hMailServer.Core.Protocols.POP3;

namespace hMailServer
{
    public class NullPop3CommandHandler : IPop3ServerCommandHandler
    {
        public Pop3CommandResult HandleQuit()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleStat()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleList()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleRetr()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleDele()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleNoop()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleRset()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleTop()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleUidl()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleUser()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandlePass()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleCapa()
        {
            throw new System.NotImplementedException();
        }

        public Pop3CommandResult HandleStls()
        {
            throw new System.NotImplementedException();
        }
    }
}