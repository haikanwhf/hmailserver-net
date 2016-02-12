using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public class Connection : IConnection, IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _networkStream;
        private MemoryStream _inboundUnprocessedStream = new MemoryStream();
        private readonly CancellationToken _cancellationToken;

        public Connection(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            _tcpClient = tcpClient;
            
            _networkStream = _tcpClient.GetStream();
            _cancellationToken = cancellationToken;
        }

        public async Task<string> ReadStringUntil(string delimiter)
        {
            byte[] dataReceived = new byte[1024];
            int readBytes = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length, _cancellationToken);

            while (readBytes > 0)
            {
                _inboundUnprocessedStream.Write(dataReceived, 0, readBytes);

                var allBytes = _inboundUnprocessedStream.ToArray();
                var data = Encoding.UTF8.GetString(allBytes);

                var index = data.IndexOf(delimiter, StringComparison.InvariantCultureIgnoreCase);

                if (index >= 0)
                {
                    int remainingStartIndex = index + delimiter.Length;
                    int remainingBytes = allBytes.Length - remainingStartIndex;

                    _inboundUnprocessedStream?.Dispose();
                    _inboundUnprocessedStream = new MemoryStream();
                    _inboundUnprocessedStream.Write(allBytes, remainingStartIndex, remainingBytes);

                    return data.Substring(0, index);
                }

                readBytes = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length, _cancellationToken);
            }

            throw new DisconnectedException();
        }

        /// <summary>
        /// Reads pending data and returns it.
        /// Any unprocessed data is returned first.
        /// Caller should dispose returned stream.
        /// </summary>
        /// <returns></returns>
        public async Task<MemoryStream> Read()
        {
            if (_inboundUnprocessedStream != null && _inboundUnprocessedStream.Length > 0)
            {
                try
                {
                    _inboundUnprocessedStream.Seek(0, SeekOrigin.Begin);
                    return _inboundUnprocessedStream;
                }
                finally
                {
                    _inboundUnprocessedStream = null;
                }
            }

            var readStream = new MemoryStream();

            byte[] dataReceived = new byte[1024 * 40];
            int readBytes = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length, _cancellationToken);

            if (readBytes == 0)
                throw new DisconnectedException();

            readStream.Write(dataReceived, 0, readBytes);
            readStream.Seek(0, SeekOrigin.Begin);

            return readStream;

        }

        //public void SkipForwardInReadStream(int skipBytes)
        //{
        //    if (skipBytes > _inboundUnprocessedStream.Length)
        //        throw new ArgumentException("skipBytes cannot exceed buffer length", nameof(skipBytes));

        //    var allBytes = _inboundUnprocessedStream.ToArray();
        //    int remainingBytes = allBytes.Length - skipBytes;

        //    _inboundUnprocessedStream?.Dispose();
        //    _inboundUnprocessedStream = new MemoryStream();
        //    _inboundUnprocessedStream.Write(allBytes, skipBytes, remainingBytes);
        //}
       
        public async Task WriteString(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            await _networkStream.WriteAsync(bytes, 0, bytes.Length, _cancellationToken);
        }

        public void Dispose()
        {
            _networkStream.Dispose();
            _inboundUnprocessedStream.Dispose();
            _tcpClient.Close();
        }
    }

}
