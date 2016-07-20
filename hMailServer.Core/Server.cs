using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using hMailServer.Core.Logging;

namespace hMailServer.Core
{
    public class Server
    {
        private readonly TcpListener _listener;
        private readonly Func<ISession> _sessionFactory;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ServerConfiguration _configuration;
        private readonly ILog _log;

        public Server(Func<ISession> sessionFactory, ILog log, ServerConfiguration serverConfiguration)
        {
            _sessionFactory = sessionFactory;
            _configuration = serverConfiguration;
            _listener = new TcpListener(serverConfiguration.IpAddress, serverConfiguration.Port);
            _log = log;
        }

        public IPEndPoint LocalEndpoint => 
            _listener.LocalEndpoint as IPEndPoint;

        public async Task RunAsync()
        {
            _listener.Start();

            while (true)
            {
                TcpClient tcpClient = null;

                try
                {
                    tcpClient = await _listener.AcceptTcpClientAsync();
                }
                catch (ObjectDisposedException)
                {
                    // When TcpListener is stopped, outstanding calls to AcceptTcpClientAsync
                    // will throw an ObjectDisposedException. When this happens, it's time to 
                    // exit.
                    return;
                }


                var connection = new Connection(tcpClient, _cancellationTokenSource.Token);

                var session = _sessionFactory();

                var sessionTask = session.HandleConnection(connection);

                SessionManager.IncreaseSessionCount(session.Protocol);

                HandleSessionAsynchronously(sessionTask, connection, session);
            }
        }

        /// <summary>
        /// When session has completed, log any error which occured and dispose resources. This is done asynchronously
        /// sincew we want to be able to start new session without previous to complete.
        /// </summary>
        private void HandleSessionAsynchronously(Task sessionTask, Connection connection, ISession session)
        {
            sessionTask.ContinueWith(f =>
            {
                if (f.IsFaulted)
                {
                    var exception = f.Exception.InnerException;

                    _log.LogException(new LogEvent()
                    {
                        LogLevel = LogLevel.Error,
                        EventType = LogEventType.Application,
                        Message = exception.Message,
                        RemoteEndpoint = connection.RemoteEndpoint,
                        SessionId = connection.SessionId,
                        Protocol = session.Protocol.ToString()
                    }, exception);
                }

                SessionManager.DecreaseSessionCount(session.Protocol);

                // Session has ended.
                connection.Dispose();
            });
       }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _listener.Stop();

            // TODO: When what?
            await Task.WhenAll();
        }
    }
}
