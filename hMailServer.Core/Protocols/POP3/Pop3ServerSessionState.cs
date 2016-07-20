namespace hMailServer.Core.Protocols.POP3
{
    class Pop3ServerSessionState
    {
        public bool HasUsername => !string.IsNullOrWhiteSpace(Username);
        public bool HasPassword => !string.IsNullOrWhiteSpace(Password);

        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsLoggedOn;

        public void Reset()
        {
            Username = null;
            Password = null;
            IsLoggedOn = false;
        }

        public bool IsCommandValid(Pop3Command command)
        {
            switch (command)
            {
                case Pop3Command.Quit:
                case Pop3Command.Help:
                case Pop3Command.Capa:
                    return true;
                case Pop3Command.User:
                    return !HasUsername;
                case Pop3Command.Pass:
                    return HasUsername && !HasPassword;
                case Pop3Command.Uidl:
                case Pop3Command.Stat:
                case Pop3Command.List:
                case Pop3Command.Retr:
                case Pop3Command.Dele:
                    return IsLoggedOn;

                default:
                    return false;
            }
        }
    }
}
