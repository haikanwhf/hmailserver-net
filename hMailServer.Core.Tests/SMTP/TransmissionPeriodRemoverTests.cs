using System;
using System.IO;
using System.Text;
using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;

namespace hMailServer.Core.Tests.SMTP
{
    [TestFixture]
    public class TransmissionPeriodRemoverTests
    {
        [Test]
        public void ProcessingEmptyStreamShouldReturnEmptyStream()
        {
            var input = new MemoryStream();
            var output = new MemoryStream();

            TransmissionPeriodRemover.Process(input, output, 0);

            Assert.AreEqual(0, output.Length);
        }

        [Test]
        public void ProcessingMoreBytesThanInputStreamShouldThrow()
        {
            var input = new MemoryStream();
            var output = new MemoryStream();

            var exception = Assert.Throws<ArgumentException>(() => TransmissionPeriodRemover.Process(input, output, 1));

            Assert.AreEqual("inputBytesToProcess", exception.ParamName);
            StringAssert.StartsWith("inputBytesToProcess > input.Length", exception.Message);
        }

        [Test]
        public void ProcessingTwoDotInStartShouldRemoveOne()
        {
            var inputString = "..";

            var outputString = ProcessInputString(inputString);

            Assert.AreEqual(".", outputString);
        }

        [Test]
        public void ProcessingOneDotInStartShouldLeaveIt()
        {
            var inputString = ".";

            var outputString = ProcessInputString(inputString);

            Assert.AreEqual(".", outputString);
        }

        [Test]
        public void ProcessingTwoDotOnNewlineShouldRemoveOne()
        {
            var inputString = "A\r\n..\r\nB";

            var outputString = ProcessInputString(inputString);

            Assert.AreEqual("A\r\n.\r\nB", outputString);
        }

        [Test]
        public void ProcessingOneDotOnNewlineShouldLeaveIt()
        {
            var inputString = "A\r\n.\r\nB";

            var outputString = ProcessInputString(inputString);

            Assert.AreEqual("A\r\n.\r\nB", outputString);
        }

        [Test]
        public void ProcessingTwoDotsInMiddleOfLineShouldLeaveBoth()
        {
            var inputString = "A\r\nA..A\r\nA";

            var outputString = ProcessInputString(inputString);

            Assert.AreEqual("A\r\nA..A\r\nA", outputString);
        }

        [Test]
        public void ProcessingTwoDotsInStartOfLineWithTrailingOtherCharactersShouldRemoveOneDot()
        {
            var inputString = "A\r\n..A\r\nA";

            var outputString = ProcessInputString(inputString);

            Assert.AreEqual("A\r\n.A\r\nA", outputString);
        }

        private static string ProcessInputString(string inputString)
        {
            var inputBytes = Encoding.UTF8.GetBytes(inputString);
            var inputMemoryStream = new MemoryStream(inputBytes);

            var output = new MemoryStream();

            TransmissionPeriodRemover.Process(inputMemoryStream, output, (int) inputMemoryStream.Length);

            var outputString = Encoding.UTF8.GetString(output.ToArray());
            return outputString;
        }
    }
}
