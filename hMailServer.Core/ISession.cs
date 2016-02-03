using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public interface ISession
    {
        void HandleConnection(IConnection connection);
    }
}
