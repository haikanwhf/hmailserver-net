using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using hMailServer.Core.Protocols.POP3;

namespace hMailServer.Core.Protocols.SMTP
{
    internal class CommandParser
    {
        // TODO: Limit host name matching?
        private static readonly Regex HeloEhloRegex = new Regex(@"^((HELO)|(EHLO)) (.{1,253})$", RegexOptions.IgnoreCase);

        private static readonly Dictionary<string, SmtpCommand> Commands = new Dictionary<string, SmtpCommand>();

        static CommandParser()
        {
            var values = Enum.GetValues(typeof(SmtpCommand));

            foreach (var value in values)
                Commands[value.ToString().ToLowerInvariant()] = (SmtpCommand)value;
        }

        public static SmtpCommand ParseCommand(string command)
        {
            command = command.ToLowerInvariant();

            var parts = command.Split(' ');

            if (parts.Length == 0)
                return SmtpCommand.Unknown;

            if (Commands.ContainsKey(parts[0]))
                return Commands[parts[0]];

            return SmtpCommand.Unknown;
        }

        public static string ParseHeloEhlo(string command)
        {
            var match = HeloEhloRegex.Match(command);

            if (!match.Success)
                return null;
            
            if (match.Groups.Count != 5)
                return null;

            return match.Groups[4].Value;
        }

        public static string ParseMailFrom(string command)
        {
            var value = command.Substring(10);
            value = value.Replace("<", "");
            value = value.Replace(">", "");

            return value.Trim();
        }

        public static string ParseRcptTo(string command)
        {
            var value = command.Substring(8);
            value = value.Replace("<", "");
            value = value.Replace(">", "");

            return value.Trim();
        }

        public static string ParseEhlo(string data)
        {
            return ParseHeloEhlo(data);
        }
    }
}
