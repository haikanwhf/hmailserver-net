using System.Text.RegularExpressions;

namespace hMailServer.Core.Protocols.SMTP
{
    internal class CommandParser
    {
        private static Regex HeloRegex = new Regex(@"^HELO ([\w\.]{1,253})$", RegexOptions.IgnoreCase);

        public static string ParseHelo(string command)
        {
            var match = HeloRegex.Match(command);

            if (!match.Success)
                return null;
            
            if (match.Groups.Count != 2)
                return null;

            return match.Groups[1].Value;
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
