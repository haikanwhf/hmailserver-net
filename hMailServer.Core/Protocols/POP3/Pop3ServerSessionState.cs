namespace hMailServer.Core.Protocols.POP3
{
    class Pop3ServerSessionState
    {
        public bool HasUsername => !string.IsNullOrWhiteSpace(Username);
        public bool HasPassword => !string.IsNullOrWhiteSpace(Password);

        public string Username { get; set; }
        public string Password { get; set; }

        public void Reset()
        {
            Username = null;
            Password = null;
        }

        public bool IsCommandValid(Pop3Command command)
        {
            switch (command)
            {
                case Pop3Command.Quit:
                    return true;
                case Pop3Command.Capa:
                    return true;
                case Pop3Command.User:
                    return !HasUsername;
                case Pop3Command.Pass:
                    return HasUsername && !HasPassword;
                case Pop3Command.Uidl:
                    return HasUsername && HasPassword;
                default:
                    return false;
            }
        }
    }
}
