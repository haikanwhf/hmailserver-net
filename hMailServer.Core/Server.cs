using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public class Server
    {
        private readonly TcpListener _listener;
        private readonly Func<ISession> _sessionFactory;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ServerConfiguration _configuration;

        public Server(Func<ISession> sessionFactory, ServerConfiguration serverConfiguration)
        {
            _sessionFactory = sessionFactory;
            _configuration = serverConfiguration;
            _listener = new TcpListener(serverConfiguration.IpAddress, serverConfiguration.Port);
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

                sessionTask.ContinueWith(f =>
                    {
                        // Session has ended.
                        connection.Dispose();
                    });
            }
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _listener.Stop();

            await Task.WhenAll();
        }
    }
}
