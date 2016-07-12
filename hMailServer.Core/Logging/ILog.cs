using System;
using hMailServer.Core.Logging;

namespace hMailServer.Core
{
    public interface ILog
    {
        void LogInfo(LogEvent logEvent);
        void LogException(LogEvent logEvent, Exception exception);
    }
}
