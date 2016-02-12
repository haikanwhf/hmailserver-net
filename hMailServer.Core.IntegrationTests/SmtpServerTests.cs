using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;

namespace hMailServer.Core.IntegrationTests
{
    [TestFixture]
    public class SmtpServerTests
    {
        [Test]
        public void TestSmtpEndToEnd()
        {
            var commandHandler = new InMemoryCommandHandler();

            Func<ISession> connectionFactory = () =>
                new SmtpServerSession(commandHandler);

            var serverConfiguration = new ServerConfiguration();

            var smtpServer = new Server(connectionFactory, serverConfiguration);
            var runTask = smtpServer.RunAsync();

            using (var message = new MailMessage())
            {
                message.From = new MailAddress("sender@example.com");
                message.To.Add(new MailAddress("recipient1@example.com"));
                message.To.Add(new MailAddress("recipient2@example.com"));
                message.Body = "Test";

                using (
                    var client = new SmtpClient(smtpServer.LocalEndpoint.Address.ToString(),
                        smtpServer.LocalEndpoint.Port))

                {
                    client.Send(message);
                }
            }

            var stopTask = smtpServer.StopAsync();

            Assert.IsTrue(stopTask.Wait(TimeSpan.FromMilliseconds(2000)));
            Assert.IsTrue(runTask.Wait(TimeSpan.FromMilliseconds(2000)));

            Assert.AreEqual("sender@example.com", commandHandler.MailFrom);
            Assert.AreEqual(2, commandHandler.Recipients.Count);
            Assert.AreEqual("recipient1@example.com", commandHandler.Recipients[0]);
            Assert.AreEqual("recipient2@example.com", commandHandler.Recipients[1]);

            string mailMessage = Encoding.UTF8.GetString(commandHandler.Body.ToArray());
            var bodyStart = mailMessage.IndexOf("\r\n\r\n", StringComparison.InvariantCultureIgnoreCase);
            var body = mailMessage.Substring(bodyStart + 4);
            Assert.AreEqual("Test\r\n\r\n", body);
        }
    }
}
