namespace hMailServer.Core.Protocols.POP3
{
    enum Pop3Command
    {
        Unknown,
        Noop,
        Stls,
        User,
        Pass,
        Help,
        Quit,
        Stat,
        List,
        Retr,
        Top,
        Rset,
        Dele,
        Uidl,
        Capa
    }
}
