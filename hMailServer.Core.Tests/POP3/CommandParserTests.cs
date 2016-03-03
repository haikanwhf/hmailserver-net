using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.POP3;
using NUnit.Framework;

namespace hMailServer.Core.Tests.POP3
{
    [TestFixture]
    public class CommandParserTests
    {
        [Test]
        public void ParsingQuitShouldSucceed()
        {
            var parseResult = CommandParser.ParseCommand("QUIT");

            Assert.AreEqual(Pop3Command.Quit, parseResult);
                ;
        }
    }
}
