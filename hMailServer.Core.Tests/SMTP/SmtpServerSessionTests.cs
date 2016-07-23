using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Logging;
using hMailServer.Core.Protocols.SMTP;
using hMailServer.Core.Tests.Common;
using Moq;
using NUnit.Framework;

namespace hMailServer.Core.Tests.SMTP
{
    [TestFixture]
    public class SmtpServerSessionTests
    {
        [Test]
        public void TestCompleteSmtpServerSession()
        {
            var commandHandlerMock = new Mock<ISmtpServerCommandHandler>();
            commandHandlerMock.Setup(f => f.HandleHelo("example.com")).Returns(SmtpCommandReply.CreateDefault250SuccessTask);
            commandHandlerMock.Setup(f => f.HandleMailFrom("knafve@example.com")).Returns(SmtpCommandReply.CreateDefault250SuccessTask);
            commandHandlerMock.Setup(f => f.HandleRcptTo("knafve@example.com")).Returns(SmtpCommandReply.CreateDefault250SuccessTask);

            commandHandlerMock.Setup(f => f.HandleData(It.IsAny<MemoryStreamWithFileBacking>()))
                .Callback((Stream stream) =>
                    VerifyMemoryStreamContents(stream, "Hello World\r\n"))
                .Returns(SmtpCommandReply.CreateDefault250SuccessTask());

            var data = "Hello World\r\n.\r\n";
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(data));

            var connectionMock = ConnectionMockFactory.Create(new string[]
                {
                    "HELO example.com",
                    "MAIL FROM: knafve@example.com",
                    "RCPT TO: knafve@example.com",
                    "DATA",
                    "QUIT"
                }, new []
                {
                    memory
                });

            SmtpServerSession session = new SmtpServerSession(commandHandlerMock.Object, new NullLog(), new SmtpServerSessionConfiguration());

            var task = session.HandleConnection(connectionMock);
            task.Wait();

            commandHandlerMock.Verify(f => f.HandleHelo("example.com"));
        }

        [Test]
        public void SmtpConversationShouldBeLogged()
        {
            var inMemoryLog = new InMemoryLog();

            var commandHandlerMock = new Mock<ISmtpServerCommandHandler>();
            commandHandlerMock.Setup(f => f.HandleHelo("example.com")).Returns(SmtpCommandReply.CreateDefault250SuccessTask);
            commandHandlerMock.Setup(f => f.HandleMailFrom("knafve@example.com")).Returns(SmtpCommandReply.CreateDefault250SuccessTask);
            commandHandlerMock.Setup(f => f.HandleRcptTo("knafve@example.com")).Returns(SmtpCommandReply.CreateDefault250SuccessTask);
            commandHandlerMock.Setup(f => f.HandleData(It.IsAny<MemoryStreamWithFileBacking>())).Returns(SmtpCommandReply.CreateDefault250SuccessTask);

            var data = "Hello World\r\n.\r\n";
            var memory = new MemoryStream(Encoding.UTF8.GetBytes(data));

            var connectionMock = ConnectionMockFactory.Create(new string[]
                {
                    "HELO example.com",
                    "MAIL FROM: knafve@example.com",
                    "RCPT TO: knafve@example.com",
                    "DATA",
                    "QUIT"
                }, new[]
                {
                    memory
                });

            SmtpServerSession session = new SmtpServerSession(commandHandlerMock.Object, inMemoryLog, new SmtpServerSessionConfiguration());

            var task = session.HandleConnection(connectionMock);
            task.Wait();

            var expectedLogEntries = new List<string>()
                {
                    "HELO example.com",
                    "MAIL FROM: knafve@example.com",
                    "RCPT TO: knafve@example.com",
                    "DATA",
                    "QUIT",

                    "250 Ok"
                };


            foreach (var expectedLogEntry in expectedLogEntries)
                Assert.IsTrue(inMemoryLog.LogEntries.Any(logEntry => logEntry.Item1.Message.Contains(expectedLogEntry)), expectedLogEntry);


        }

        private bool VerifyMemoryStreamContents(Stream stream, string expectedText)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var actualBytes = memoryStream.ToArray();
            var actualText = Encoding.UTF8.GetString(actualBytes);

            return expectedText == actualText;
        }
    }
}
