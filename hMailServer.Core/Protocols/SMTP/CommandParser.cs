namespace hMailServer.Core.Protocols.SMTP
{
    class CommandParser
    {
        public static string ParseHelo(string command)
        {
            return command.Substring(4);
        }

        public static string ParseMailFrom(string command)
        {
            var value = command.Substring(10);
            value = value.Replace("<", "");
            value = value.Replace(">", "");

            return value;
        }

        public static string ParseRcptTo(string command)
        {
            var value = command.Substring(8);
            value = value.Replace("<", "");
            value = value.Replace(">", "");

            return value;
        }
    }
}
