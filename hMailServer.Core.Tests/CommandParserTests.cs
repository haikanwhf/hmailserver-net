using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;


namespace hMailServer.Core.Tests
{
    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        public void ParsingHeloWithMissingHostnameShouldSucceed()
        {
            Assert.IsNull(CommandParser.ParseHelo("HELO"));
        }

        [Test]
        public void ParsingHeloWithTrailingSpaceAndMissingHostnameShouldSucceed()
        {
            Assert.IsNull(CommandParser.ParseHelo("HELO "));
        }

        [Test]
        public void ParsingHeloWithHostnameWithLeadingAndTrailingSpaceShouldTrim()
        {
            Assert.AreEqual("example.com", CommandParser.ParseHelo("HELO example.com"));
        }
    }
}
