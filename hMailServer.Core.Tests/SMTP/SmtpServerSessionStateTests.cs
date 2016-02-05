using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;

namespace hMailServer.Core.Tests.SMTP
{
    [TestFixture]
    public class SmtpServerSessionStateTests
    {
        [Test]
        public void MailFromBeforeHeloShouldFail()
        {
            var state = new SmtpServerSessionState()
                {
                    HasHelo = false,
                    HasMailFrom = false,
                    HasRcptTo = false
                };

            Assert.IsFalse(state.IsCommandValid(SmtpCommand.MailFrom));
        }

        [Test]
        public void MailFromAfterHeloShouldFail()
        {
            var state = new SmtpServerSessionState()
            {
                HasHelo = true,
                HasMailFrom = false,
                HasRcptTo = false
            };

            Assert.True(state.IsCommandValid(SmtpCommand.MailFrom));
        }

        [Test]
        public void RcptToBeforeMailFromShouldFail()
        {
            var state = new SmtpServerSessionState()
            {
                HasHelo = true,
                HasMailFrom = false,
                HasRcptTo = false
            };

            Assert.IsFalse(state.IsCommandValid(SmtpCommand.RcptTo));
        }

        [Test]
        public void RcptToAfterMailFromShouldSucceed()
        {
            var state = new SmtpServerSessionState()
            {
                HasHelo = true,
                HasMailFrom = true,
                HasRcptTo = false
            };

            Assert.True(state.IsCommandValid(SmtpCommand.RcptTo));
        }

        [Test]
        public void DataBeforeRcptToShouldFail()
        {
            var state = new SmtpServerSessionState()
            {
                HasHelo = true,
                HasMailFrom = true,
                HasRcptTo = false
            };

            Assert.IsFalse(state.IsCommandValid(SmtpCommand.Data));
        }

        [Test]
        public void DataAfterRpptToShouldSucceed()
        {
            var state = new SmtpServerSessionState()
            {
                HasHelo = true,
                HasMailFrom = true,
                HasRcptTo = true
            };

            Assert.True(state.IsCommandValid(SmtpCommand.Data));
        }


    }
}
