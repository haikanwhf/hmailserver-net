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

            throw new DisconnectedException();
        }

        public async Task<MemoryStream> Read()
        {
            if (_inboundStream.Length > 0)
            {
                try
                {
                    _inboundStream.Seek(0, SeekOrigin.Begin);
                    return _inboundStream;
                }
                finally
                {
                    _inboundStream = new MemoryStream();
                }
            }
            
            byte[] dataReceived = new byte[50];
            int readBytes = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length);

            if (readBytes == 0)
                throw new DisconnectedException();
            
            _inboundStream.Seek(0, SeekOrigin.End);
            _inboundStream.Write(dataReceived, 0, readBytes);
            _inboundStream.Seek(0, SeekOrigin.Begin);

            return _inboundStream;
        }

        public void SkipForwardInReadStream(int skipBytes)
        {
            if (skipBytes > _inboundStream.Length)
                throw new ArgumentException("skipBytes cannot exceed buffer length", nameof(skipBytes));

            var allBytes = _inboundStream.ToArray();
            int remainingBytes = allBytes.Length - skipBytes;

            _inboundStream = new MemoryStream();
            _inboundStream.Write(allBytes, skipBytes, remainingBytes);
        }



        public async Task WriteString(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            await _networkStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }

}
