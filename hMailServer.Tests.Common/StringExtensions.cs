using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Tests.Common
{
    public static class StringExtensions
    {
        public static Attachment ToAttachment(this string data, string name = "attachment.txt")
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            streamWriter.Write(data);
            streamWriter.Flush();
            memoryStream.Position = 0;

            return new Attachment(memoryStream, name);

        }
    }
}
