using System;
using hMailServer.Core;
using hMailServer.Core.Logging;
using hMailServer.Core.Protocols.POP3;
using hMailServer.Core.Protocols.SMTP;

namespace hMailServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<ISession> smtpSessionFactory = () => 
                new SmtpServerSession(new NullSmtpCommandHandler(), new NullLog(), new SmtpServerSessionConfiguration());

            var serverConfiguration = new ServerConfiguration()
                {
                    Port = 25
                };

            var smtpServer = new Server(smtpSessionFactory, serverConfiguration);
            var smtpRunTask = smtpServer.RunAsync();

            Func<ISession> pop3SessionFactory = () =>
             new Pop3ServerSession(new NullPop3CommandHandler(), new NullLog(), new Pop3ServerSessionConfiguration());

            var pop3ServerConfiguration = new ServerConfiguration()
                {
                    Port = 250
                };

            var pop3Server = new Server(pop3SessionFactory, pop3ServerConfiguration);
            var pop3RunTask = pop3Server.RunAsync();

            Console.WriteLine("Server running Press Enter to exit...");
            Console.ReadLine();

            Console.WriteLine("Shutting down");

            var smtpStopTask = smtpServer.StopAsync();
            var pop3StopTask = pop3Server.StopAsync();

        }

    }
}
