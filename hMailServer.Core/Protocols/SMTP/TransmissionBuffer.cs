using System;
using System.IO;

namespace hMailServer.Core.Protocols.SMTP
{
    public class TransmissionBuffer
    {
        private readonly MemoryStream _buffer;
        private bool _transmissionEnded;
        private readonly Stream _target;
        private const int MaxLineLength = 100000;

        public TransmissionBuffer(Stream target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            _buffer = new MemoryStream();
            _target = target;
            _transmissionEnded = false;
        }

        public void Append(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _buffer.Seek(0, SeekOrigin.End);
            stream.WriteTo(_buffer);

            _transmissionEnded = HasTransmissionEndedIndicator();

            if (FlushRequired)
            {
                Flush();
            }
        }

        public bool FlushRequired => 
            _transmissionEnded || _buffer.Length > 40000;

        public bool TransmissionEnded => _transmissionEnded;

        private bool HasTransmissionEndedIndicator()
        {
            if (_buffer.Length >= 3)
            {
                const int startByteIndicatorSize = 3;

                var startBytes = new byte[startByteIndicatorSize];

                _buffer.Seek(0, SeekOrigin.Begin);
                if (_buffer.Read(startBytes, 0, startBytes.Length) == startByteIndicatorSize)
                {
                    if (startBytes[0] == '.' &&
                        startBytes[1] == '\r' &&
                        startBytes[2] == '\n')
                    {
                        return true;
                    }
                }
            }

            if (_buffer.Length >= 5)
            {
                const int endByteIndicatorSize = 5;

                var endBytes = new byte[endByteIndicatorSize];

                _buffer.Seek(-endByteIndicatorSize, SeekOrigin.End);

                if (_buffer.Read(endBytes, 0, endBytes.Length) == endByteIndicatorSize)
                {
                    if (endBytes[0] == '\r' &&
                        endBytes[1] == '\n' &&
                        endBytes[2] == '.' &&
                        endBytes[3] == '\r' &&
                        endBytes[4] == '\n')
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Flush()
        {
            /*
                If we found a newline, send anything up until that.
                If we're forcing a send, send all we got
                If we found no newline in the stream, the message is malformed according to RFC2821 (max 1000 chars per line). 
             */

            byte[] bytesToScanForNewline;

            if (_buffer.Length > MaxLineLength)
            {
                bytesToScanForNewline = new byte[MaxLineLength];
                _buffer.Seek(-MaxLineLength, SeekOrigin.End);
                _buffer.Read(bytesToScanForNewline, (int) (_buffer.Length - MaxLineLength), MaxLineLength);
            }
            else
            {
                bytesToScanForNewline = new byte[_buffer.Length];

                _buffer.Seek(0, SeekOrigin.Begin);
                _buffer.Read(bytesToScanForNewline, 0, (int) _buffer.Length);
            }

            int flushPosition = (int) _buffer.Length;
            for (int i = bytesToScanForNewline.Length - 1; i >= 0; i--)
            {
                if (bytesToScanForNewline[i] == '\n')
                {
                    // Send all up until this position.
                    int skippedBytes = bytesToScanForNewline.Length - i;

                    // include the newline character in the flushed data
                    flushPosition = (int) _buffer.Length - skippedBytes + 1; 
                }
            }

            _buffer.Seek(0, SeekOrigin.Begin);

            TransmissionPeriodRemover.Process(_buffer, _target, flushPosition);
        }
    }
}
