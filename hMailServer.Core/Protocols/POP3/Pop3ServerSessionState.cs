using hMailServer.Core.Protocols.SMTP;

namespace hMailServer.Core.Protocols.POP3
{
    class Pop3ServerSessionState
    {
        public bool HasUsername { get; set; }
        public bool HasPassword { get; set; }

        public void Reset()
        {
            HasUsername = false;
            HasPassword = false;
        }

        public bool IsCommandValid(Pop3Command command)
        {
            switch (command)
            {
                case Pop3Command.Quit:
                    return true;
                case Pop3Command.Uidl:
                    return HasUsername && HasPassword;
                default:
                    return false;
            }
        }
    }
}
