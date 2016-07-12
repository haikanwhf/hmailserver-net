using System.Net;

namespace hMailServer.Core.Logging
{
    public class LogEvent
    {
        public LogLevel LogLevel { get; set; }
        public string Protocol { get; set; }
        public LogEventType EventType { get; set; }
        public IPEndPoint RemoteEndpoint { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
    }
}
