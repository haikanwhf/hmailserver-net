using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Logging;

namespace hMailServer.Core.Tests
{
    class InMemoryLog : ILog
    {
        public List<Tuple<LogEvent, Exception>> LogEntries = new List<Tuple<LogEvent, Exception>>(); 

        public void LogInfo(LogEvent logEvent)
        {
            LogEntries.Add(new Tuple<LogEvent, Exception>(logEvent, null));
        }

        public void LogException(LogEvent logEvent, Exception ex)
        {
            LogEntries.Add(new Tuple<LogEvent, Exception>(logEvent, ex));
        }
    }
}
