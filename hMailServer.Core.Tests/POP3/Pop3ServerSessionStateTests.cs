using hMailServer.Core.Protocols.POP3;
using hMailServer.Core.Protocols.SMTP;
using NUnit.Framework;

namespace hMailServer.Core.Tests.POP3
{
    [TestFixture]
    public class Pop3ServerSessionStateTests
    {
        [Test]
        public void UidlBeforeLogonShouldFail()
        {
            var state = new Pop3ServerSessionState()
                {
                    IsLoggedOn = false,
                };

            Assert.IsFalse(state.IsCommandValid(Pop3Command.Uidl));
        }

        [Test]
        public void UidlAfterLogonShouldSucceed()
        {
            var state = new Pop3ServerSessionState()
                {
                    IsLoggedOn = true,
                };

            Assert.IsTrue(state.IsCommandValid(Pop3Command.Uidl));
        }
    }
}
