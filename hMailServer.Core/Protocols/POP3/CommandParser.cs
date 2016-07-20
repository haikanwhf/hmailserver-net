using System;
using System.Collections.Generic;

namespace hMailServer.Core.Protocols.POP3
{
    internal class CommandParser
    {
        private static readonly Dictionary<string, Pop3Command> Commands = new Dictionary<string, Pop3Command>();

        static CommandParser()
        {
            var values = Enum.GetValues(typeof (Pop3Command));

            foreach (var value in values)
                Commands[value.ToString().ToLowerInvariant()] = (Pop3Command) value;
        }

        public static Pop3Command ParseCommand(string command)
        {
            command = command.ToLowerInvariant();

            var parts = command.Split(' ');

            if (parts.Length == 0)
                return Pop3Command.Unknown;

            if (Commands.ContainsKey(parts[0]))
                return Commands[parts[0]];

            
            return Pop3Command.Unknown;
        }

        public static string ParseUser(string command)
        {
            var parts = command.Split(' ');

            if (parts.Length != 2)
                return null;

            return parts[1].Trim();
        }

        public static string ParsePass(string command)
        {
            var parts = command.Split(' ');

            if (parts.Length != 2)
                return null;

            return parts[1].Trim();
        }
    }
}
