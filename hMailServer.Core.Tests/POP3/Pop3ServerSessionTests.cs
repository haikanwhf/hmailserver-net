using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Logging;
using hMailServer.Core.Protocols.POP3;
using hMailServer.Core.Protocols.SMTP;
using hMailServer.Core.Tests.Common;
using Moq;
using NUnit.Framework;

namespace hMailServer.Core.Tests.POP3
{
    [TestFixture]
    class Pop3ServerSessionTests
    {
        [Test]
        public void TestImmediateQuit()
        {
            var commandHandlerMock = new Mock<IPop3ServerCommandHandler>();
            commandHandlerMock.Setup(f => f.HandleQuit()).Returns(new Pop3CommandResult(true, "Ok"));
            
            var connectionMock = ConnectionMockFactory.Create(new[]
                {
                    "QUIT"
                }, new List<MemoryStream>());

            Pop3ServerSession session = new Pop3ServerSession(commandHandlerMock.Object, new NullLog(), new Pop3ServerSessionConfiguration());

            var task = session.HandleConnection(connectionMock);
            task.Wait();

            commandHandlerMock.Verify(f => f.HandleQuit());
        }

    }
}
