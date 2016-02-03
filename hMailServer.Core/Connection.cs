using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public class Connection : IConnection
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;
        private MemoryStream _inboundStream = new MemoryStream();

        public Connection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;

            _networkStream = _tcpClient.GetStream();
        }

        public async Task<string> ReadStringUntil(string delimiter)
        {
            byte[] dataReceived = new byte[50];
            int readBytes = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length);

            while (readBytes > 0)
            {
                _inboundStream.Write(dataReceived, 0, readBytes);

                var allBytes = _inboundStream.ToArray();
                var data = Encoding.UTF8.GetString(allBytes);

                var index = data.IndexOf(delimiter);

                if (index >= 0)
                {
                    int remainingStartIndex = index + delimiter.Length;
                    int remainingBytes = allBytes.Length - remainingStartIndex;

                    _inboundStream = new MemoryStream();
                    _inboundStream.Write(allBytes, remainingStartIndex, remainingBytes);

                    return data.Substring(0, index);
                }

                readBytes = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length);
            }

            throw new Exception("Expected data not received.");
        }

        public async Task<string> ReadUntilNewLine()
        {
            return await ReadStringUntil("\r\n");
        }

        public async Task WriteString(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            await _networkStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }

}
