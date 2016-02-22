using System;
using System.IO;

namespace hMailServer.Core
{
    public class MemoryStreamWithFileBacking : Stream
    {
        private readonly MemoryStream _memoryStream;
        private FileStream _fileStream;
        private readonly string _backingFilePath;
        private readonly int _memoryBufferMaxSize;

        public MemoryStreamWithFileBacking(int memoryBufferMaxSize)
        {
            _memoryStream = new MemoryStream();
            _backingFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _memoryBufferMaxSize = memoryBufferMaxSize;
        }

        internal string BackingFilePath => _backingFilePath;

        private Stream CurrentStream
        {
            get
            {
                if (_fileStream != null)
                    return _fileStream;
                
                return _memoryStream;
            }
        }

        public override void Flush()
        {
            CurrentStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return CurrentStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            CurrentStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return CurrentStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_fileStream == null)
            {
                if (_memoryStream.Length + count > _memoryBufferMaxSize)
                {
                    // Swap from memory to file
                    _fileStream = File.Open(_backingFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);

                    _memoryStream.Seek(0, SeekOrigin.Begin);
                    _memoryStream.WriteTo(_fileStream);
                    _memoryStream.Dispose();
                }
            }

            CurrentStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            _fileStream?.Dispose();
            _memoryStream?.Dispose();
            File.Delete(_backingFilePath);
            base.Dispose(disposing);
        }

        public override bool CanRead => CurrentStream.CanRead;
        public override bool CanSeek => CurrentStream.CanSeek;
        public override bool CanWrite => CurrentStream.CanWrite;
        public override long Length => CurrentStream.Length;

        public override long Position
        {
            get { return CurrentStream.Position;  }
            set { CurrentStream.Position = value; }
        }
    }
}
