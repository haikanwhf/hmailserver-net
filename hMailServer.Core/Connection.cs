using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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

        private Stream _transferStream;

        private TimeSpan _currentTimeout = TimeSpan.FromSeconds(30);

        public Connection(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            _tcpClient = tcpClient;
            
            _networkStream = _tcpClient.GetStream();
            _transferStream = _networkStream;
            _cancellationToken = cancellationToken;
        }

        private Task CreateTimeoutTask()
        {
            return Task.Delay(_currentTimeout, _cancellationToken);
        }

        public void SetTimeout(TimeSpan timeout)
        {
            _currentTimeout = timeout;
        }

        public async Task<string> ReadStringUntil(string delimiter)
        {
            byte[] dataReceived = new byte[1024];

            while (true)
            {
                var timeoutTask = CreateTimeoutTask();
                Task<int> readBytesTask = _transferStream.ReadAsync(dataReceived, 0, dataReceived.Length,
                    _cancellationToken);

                await Task.WhenAny(timeoutTask, readBytesTask);

                if (readBytesTask.IsCompleted)
                {
                    int readBytes = readBytesTask.Result;

                    if (readBytes <= 0)
                        throw new DisconnectedException();

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

                    continue;
                }

                if (timeoutTask.IsCompleted)
                {
                    HandleTimeout();
                }

            }
        }

        public Task<MemoryStream> Read(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        private void HandleTimeout()
        {
            _networkStream.Close();
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

            var timeoutTask = CreateTimeoutTask();

            var readStream = new MemoryStream();

            byte[] dataReceived = new byte[1024 * 40];
            Task<int> readTask = _transferStream.ReadAsync(dataReceived, 0, dataReceived.Length, _cancellationToken);

            await Task.WhenAny(timeoutTask, readTask);

            if (readTask.IsCompleted)
            {
                int bytesRead = readTask.Result;

                if (bytesRead <= 0)
                    throw new DisconnectedException();

                readStream.Write(dataReceived, 0, bytesRead);
                readStream.Seek(0, SeekOrigin.Begin);

                return readStream;
            }

            HandleTimeout();

            return null;
        }
       
       
        public async Task WriteString(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            var timeoutTask = CreateTimeoutTask();

            var writeTask = _transferStream.WriteAsync(bytes, 0, bytes.Length, _cancellationToken);

            await Task.WhenAny(timeoutTask, writeTask);

            if (timeoutTask.IsCompleted)
            {
                HandleTimeout();
            }
        }

        public async Task SslHandshakeAsServer(X509Certificate2 certificate)
        {
            var sslStream = new SslStream(_transferStream);

            var timeoutTask = Task.Delay(_currentTimeout, _cancellationToken);
            var handshakeTask = sslStream.AuthenticateAsServerAsync(certificate);

            await Task.WhenAny(timeoutTask, handshakeTask);

            if (timeoutTask.IsCompleted)
            {
                HandleTimeout();
            }

            _transferStream = sslStream;
        }

        public void Dispose()
        {
            _transferStream.Dispose();
            _networkStream.Dispose();
            _inboundUnprocessedStream.Dispose();
            _tcpClient.Close();
        }
    }

}
