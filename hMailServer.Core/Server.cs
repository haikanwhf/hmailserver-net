using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public class Server
    {
        private readonly TcpListener _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 25);
        private readonly Func<ISession> _sessionFactory;

        public Server(Func<ISession> sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task Start()
        {
            _listener.Start();

            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();

                var connection = new Connection(tcpClient);

                var session = _sessionFactory();

                session.HandleConnection(connection);
            }
        }
    }
}
