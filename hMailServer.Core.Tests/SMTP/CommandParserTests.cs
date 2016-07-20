using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;

namespace hMailServer.Core.Tests.SMTP
{
    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        public void ParsingHeloShouldSucceed()
        {
            Assert.AreEqual(SmtpCommand.Helo, CommandParser.ParseCommand("HELO example.com"));
        }

        [Test]
        public void ParsingEhloShouldSucceed()
        {
            Assert.AreEqual(SmtpCommand.Ehlo, CommandParser.ParseCommand("EHLO example.com"));
        }

        [Test]
        public void ParsingMailFromShouldSucceeed()
        {
            Assert.AreEqual(SmtpCommand.Mail, CommandParser.ParseCommand("MAIL FROM: test@example.com"));
        }

        [Test]
        public void ParsingRcptToShouldSucceeed()
        {
            Assert.AreEqual(SmtpCommand.Rcpt, CommandParser.ParseCommand("RCPT TO: test@example.com"));
        }

        [Test]
        public void ParsingDataShouldSucceed()
        {
            Assert.AreEqual(SmtpCommand.Data, CommandParser.ParseCommand("DATA"));
        }

        [Test]
        public void ParsingQuitShouldSucceed()
        {
            Assert.AreEqual(SmtpCommand.Quit, CommandParser.ParseCommand("QUIT"));
        }

        [Test]
        public void ParsingHeloWithMissingHostnameShouldSucceed()
        {
            Assert.IsNull(CommandParser.ParseHeloEhlo("HELO"));
        }

        [Test]
        public void ParsingHeloWithTrailingSpaceAndMissingHostnameShouldSucceed()
        {
            Assert.IsNull(CommandParser.ParseHeloEhlo("HELO "));
        }

        [Test]
        public void ParsingHeloWithHostnameWithLeadingAndTrailingSpaceShouldTrim()
        {
            Assert.AreEqual("example.com", CommandParser.ParseHeloEhlo("HELO example.com"));
        }
    }
}
