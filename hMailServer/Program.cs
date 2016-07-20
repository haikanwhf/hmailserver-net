using System;
using hMailServer.Application;
using hMailServer.Configuration;
using hMailServer.Core;
using hMailServer.Core.Protocols.POP3;
using hMailServer.Core.Protocols.SMTP;
using hMailServer.Delivery;
using hMailServer.Protocols.POP3;
using hMailServer.Protocols.SMTP;
using StructureMap;

namespace hMailServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new Log();

            var config = ServiceConfigurationReader.Read();
            
            var container = new Container(new DependencyRegistry(config));

            var smtpServerSessionConfiguration = new SmtpServerSessionConfiguration
                {
                    TempDirectory = config.TempDirectory
                };

            Func<ISession> smtpSessionFactory = () => 
                new SmtpServerSession(new SmtpServerCommandHandler(container), log, smtpServerSessionConfiguration);

            var serverConfiguration = new ServerConfiguration()
                {
                    Port = 25
                };

            var smtpServer = new Server(smtpSessionFactory, log, serverConfiguration);
            var smtpRunTask = smtpServer.RunAsync();

            Func<ISession> pop3SessionFactory = () =>
             new Pop3ServerSession(new Pop3ServerCommandHandler(container), log, new Pop3ServerSessionConfiguration());

            var pop3ServerConfiguration = new ServerConfiguration()
                {
                    Port = 110
                };



            var deliverer = new MessageDeliverer(container);
            var delivererTask = deliverer.RunAsync();





            var pop3Server = new Server(pop3SessionFactory, log, pop3ServerConfiguration);
            var pop3RunTask = pop3Server.RunAsync();

            log.LogApplicationInfo("Server running...");


            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

            log.LogApplicationInfo("Shutting down");

            var smtpStopTask = smtpServer.StopAsync();
            var pop3StopTask = pop3Server.StopAsync();

        }

    }
}
