using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
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
        
        private CancellationToken _cancellationToken;
        private Stream _transferStream;

        private TimeSpan _currentTimeout = TimeSpan.FromSeconds(30);

        public Connection(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            _tcpClient = tcpClient;
            _networkStream = _tcpClient.GetStream();
            _transferStream = _networkStream;
            
            _cancellationToken = cancellationToken;

            SessionId = SessionIdGenerator.Generate();
        }
        
        public void SetTimeout(TimeSpan timeout)
        {
            _currentTimeout = timeout;
        }

        public IPEndPoint RemoteEndpoint => (IPEndPoint) _tcpClient.Client.RemoteEndPoint;

        public string SessionId { get; }

        public async Task<string> ReadStringUntil(string delimiter)
        {
            byte[] dataReceived = new byte[1024];

            while (true)
            {
                var readBytes = await ExecuteWithTimeout(() =>
                    {
                        return _transferStream.ReadAsync(dataReceived, 0, dataReceived.Length, _cancellationToken);
                    });
                
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
            }
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
            
            byte[] dataReceived = new byte[1024 * 40];

            var bytesRead = await ExecuteWithTimeout<int>(() =>
                {
                    return _transferStream.ReadAsync(dataReceived, 0, dataReceived.Length, _cancellationToken);
                });

            var readStream = new MemoryStream();
            
            if (bytesRead <= 0)
                throw new DisconnectedException();

            readStream.Write(dataReceived, 0, bytesRead);
            readStream.Seek(0, SeekOrigin.Begin);

            return readStream;
        }

        public async Task ExecuteWithTimeout(Task func)
        {
            await ExecuteWithTimeout<int>(async () =>
            {
                await func;

                return 0;
            });
        }

        public async Task<T> ExecuteWithTimeout<T>(Func<Task<T>> func)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Task.Delay(_currentTimeout, timeoutCancellationTokenSource.Token);

            var task = func();

            await Task.WhenAny(timeoutTask, task);

            if (timeoutTask.IsCompleted)
                throw new DisconnectedException();

            // Cancel the Task.Delay to prevent it from consuming memory until it completes.
            timeoutCancellationTokenSource.Cancel();

            return task.Result;
        }

        public async Task WriteString(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            await ExecuteWithTimeout(async () =>
                {
                    await _transferStream.WriteAsync(bytes, 0, bytes.Length, _cancellationToken);

                    return 0;
                });
        }

        public async Task SslHandshakeAsServer(X509Certificate2 certificate)
        {
            var sslStream = new SslStream(_transferStream);

            await ExecuteWithTimeout(async () =>
                {
                    await sslStream.AuthenticateAsServerAsync(certificate);

                    return 0;
                });

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
