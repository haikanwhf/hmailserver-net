using System;
using hMailServer.Core;
using hMailServer.Core.Protocols.SMTP;

namespace hMailServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<ISession> connectionFactory = () => 
                new SmtpServerSession(new NullCommandHandler());

            var smtpServer = new Server(connectionFactory);

            var task = smtpServer.Start();

            task.Wait();
        }

    }
}
