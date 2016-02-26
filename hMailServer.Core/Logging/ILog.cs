using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Logging;

namespace hMailServer.Core
{
    public interface ILog
    {
        void LogInfo(LogEvent logEvent);
        void LogException(LogEvent logEvent, Exception ex);
    }
}
