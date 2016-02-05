using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;

namespace hMailServer.Core.Tests.SMTP
{
    [TestFixture]
    public class TransmissionBufferTests
    {
        [Test]
        public void TransmissionShouldNotBeMarkedAsEndedIfNoDotOnNewLineIsReached()
        {
            var target = new MemoryStream();

            var data = "Hello World\r\n";
            var bytes = Encoding.UTF8.GetBytes(data);

            var transmissionBuffer = new TransmissionBuffer(target);
            transmissionBuffer.Append(new MemoryStream(bytes));

            Assert.IsFalse(transmissionBuffer.TransmissionEnded);
        }

        [Test]
        public void TransmissionShouldBeMarkedAsEndedWhenDotOnNewLineIsReached()
        {
            var target = new MemoryStream();

            var data = "Hello World\r\n.\r\n";
            var bytes = Encoding.UTF8.GetBytes(data);

            var transmissionBuffer = new TransmissionBuffer(target);
            transmissionBuffer.Append(new MemoryStream(bytes));

            Assert.IsTrue(transmissionBuffer.TransmissionEnded);
        }

        [Test]
        public void TransmissionShouldBeFlushedWhenEnded()
        {
            var target = new MemoryStream();

            var data = "Hello World\r\n.\r\n";
            var bytes = Encoding.UTF8.GetBytes(data);

            var transmissionBuffer = new TransmissionBuffer(target);
            transmissionBuffer.Append(new MemoryStream(bytes));

            target.Seek(0, SeekOrigin.Begin);

            var receivedData = Encoding.UTF8.GetString(target.ToArray());

            Assert.AreEqual("Hello World\r\n", receivedData);
        }
    }
}
