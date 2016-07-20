using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.POP3;
using hMailServer.Repository;
using StructureMap;

namespace hMailServer.Protocols.POP3
{
    class Pop3ServerCommandHandler : IPop3ServerCommandHandler
    {
        private IContainer _container;

        public Pop3ServerCommandHandler(IContainer container)
        {
            _container = container;
        }

        public Task<Pop3CommandResult> HandleQuit()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleStat()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleList()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleRetr()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleDele()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleNoop()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleRset()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleTop()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleUidl()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleUser(string username)
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public async Task<Pop3CommandResult> HandlePass(string username, string password)
        {
            var accountRepository = _container.GetInstance<IAccountRepository>();

            var account = await accountRepository.ValidatePassword(username, password);

            if (account == null)
                return new Pop3CommandResult(false, "Invalid username or password");

            return Pop3CommandResult.CreateDefaultSuccess();
        }

        public Task<Pop3CommandResult> HandleCapa()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleStls()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }
    }
}
