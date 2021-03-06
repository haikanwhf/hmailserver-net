﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core;
using hMailServer.Core.Logging;
using hMailServer.Core.Protocols.SMTP;

namespace hMailServer.Smtp.AzureBlobStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<ISession> connectionFactory = () =>
                new SmtpServerSession(new AzureBlobStorageSmtpServerCommandHandler(), new NullLog(), new SmtpServerSessionConfiguration());

            var smtpServer = new Server(connectionFactory, new NullLog(), new ServerConfiguration());

            var task = smtpServer.RunAsync();

            task.Wait();
        }

    }
}
