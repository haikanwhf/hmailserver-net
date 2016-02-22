using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Tests.Common
{
    public static class RandomStringGenerator
    {
        public static string RandomString(int length)
        {
            const string chars = "abcdefghjklmnopqrstuvxyz1234567890";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomString2(int length)
        {
            const string chars = "abcdefghjklmnopqrstuvxyz1234567890";
            var random = new Random();
            
            var builder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                builder.Append(chars[random.Next(chars.Length)]);

            return builder.ToString();
        }
    }
}
