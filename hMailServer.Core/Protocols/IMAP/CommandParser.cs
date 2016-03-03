using System;

namespace hMailServer.Core.Protocols.IMAP
{
    internal class CommandParser
    {
        public static ImapCommand ParseCommand(string command)
        {
            ImapCommand imapCommand;
            
            if (!Enum.TryParse(command, true, out imapCommand))
                imapCommand = ImapCommand.Unknown;

            return imapCommand;
        }
       
    }
}
