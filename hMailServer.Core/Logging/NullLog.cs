using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Logging
{
    public class NullLog : ILog
    {
        public void LogInfo(LogEvent @event)
        {

        }

        public void LogException(LogEvent @event, Exception ex)
        {

        }
    }
}
