using System.Text.RegularExpressions;

namespace hMailServer.Core.Protocols.SMTP
{
    internal class CommandParser
    {
        private static readonly Regex HeloRegex = new Regex(@"^HELO ([\w\.]{1,253})$", RegexOptions.IgnoreCase);

        public static SmtpCommand ParseCommand(string command)
        {
            command = command.ToLowerInvariant();

            if (command.StartsWith("helo"))
                return SmtpCommand.Helo;
            if (command.StartsWith("ehlo"))
                return SmtpCommand.Ehlo;
            if (command.StartsWith("mail from"))
                return SmtpCommand.MailFrom;
            if (command.StartsWith("rcpt to"))
                return SmtpCommand.RcptTo;
            if (command.StartsWith("data"))
                return SmtpCommand.Data;
            if (command.StartsWith("quit"))
                return SmtpCommand.Quit;

            return SmtpCommand.Unknown;
        }

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

        public static string ParseEhlo(string data)
        {
            throw new System.NotImplementedException();
        }
    }
}
