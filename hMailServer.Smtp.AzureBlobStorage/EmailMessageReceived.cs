using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Smtp.AzureBlobStorage
{
    public class EmailMessageReceived
    {
        public EmailMessageReceived()
        {
            Recipients = new List<string>();
        }

        public string BlobUri { get; set; }

        public string From { get; set; }
        public List<string> Recipients { get; set; }
    }
}
