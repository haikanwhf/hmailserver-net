using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public interface IConnection
    {
        Task<string> ReadStringUntil(string delimiter);
        Task<MemoryStream> Read();
        Task WriteString(string data);
        void SkipForwardInReadStream(int skipBytes);
    }
}
