using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace hMailServer.Core.Tests
{
    [TestFixture]
    public class BufferedFileStreamTests
    {
        [Test]
        public void MemoryShouldNotBeFlushUntilBufferLimitReached()
        {
            using (var bufferedFileStream = new MemoryStreamWithFileBacking(6))
            {
                bufferedFileStream.Write(Encoding.UTF8.GetBytes("HELLO"), 0, 5);
                bufferedFileStream.Flush();
                
                Assert.IsFalse(File.Exists(bufferedFileStream.BackingFilePath));
            }

        }

        [Test]
        public void MemoryShouldBeFlushedWhenBufferLimitExceeded()
        {
            using (var bufferedFileStream = new MemoryStreamWithFileBacking(4))
            {
                bufferedFileStream.Write(Encoding.UTF8.GetBytes("HELLO"), 0, 5);
                bufferedFileStream.Flush();

                Assert.IsTrue(File.Exists(bufferedFileStream.BackingFilePath));
            }
        }

        [Test]
        public void WhenSwappingFromMemoryBufferToFileBufferMemoryBufferShouldBeMigrated()
        {
            using (var bufferedFileStream = new MemoryStreamWithFileBacking(6))
            {
                bufferedFileStream.Write(Encoding.UTF8.GetBytes("HELLO"), 0, 5);
                bufferedFileStream.Write(Encoding.UTF8.GetBytes(" WORLD"), 0, 6);
                bufferedFileStream.Flush();
                Assert.IsTrue(File.Exists(bufferedFileStream.BackingFilePath));
            }
        }

        [Test]
        public void BackingFileShouldBeRemovedWhenDisposed()
        {
            string backingFileName;

            using (var bufferedFileStream = new MemoryStreamWithFileBacking(1))
            {
                bufferedFileStream.Write(Encoding.UTF8.GetBytes("HELLO"), 0, 5);
                bufferedFileStream.Flush();

                backingFileName = bufferedFileStream.BackingFilePath;

                Assert.IsTrue(File.Exists(backingFileName));
            }

            Assert.IsFalse(File.Exists(backingFileName));
        }
    }
}
