using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public interface IConnection
    {
        Task<string> ReadStringUntil(string delimiter);
        Task<string> ReadUntilNewLine();
        Task WriteString(string data);
    }
}
