using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;

namespace hMailServer.Core.Tests.Common
{
    public static class ConnectionMockFactory
    {
        public static IConnection Create(IEnumerable<string> linesToReturn, IEnumerable<MemoryStream> memoryStreamsToRead)
        {
            var connectionMock = new Mock<IConnection>();

            var lineQueue = new Queue<Task<string>>();

            foreach (var line in linesToReturn)
            {
                var taskReturningString = new Task<string>(() => line);

                taskReturningString.RunSynchronously();
                taskReturningString.Wait();

                lineQueue.Enqueue(taskReturningString);
            }

            var readQueue = new Queue<Task<MemoryStream>>();

            foreach (var stream in memoryStreamsToRead)
            {
                var taskReturningMemoryStream = new Task<MemoryStream>(() => stream);

                taskReturningMemoryStream.RunSynchronously();
                taskReturningMemoryStream.Wait();

                readQueue.Enqueue(taskReturningMemoryStream);
            }

            connectionMock.Setup(o => o.ReadStringUntil(It.IsAny<string>()))
                .Returns(() => lineQueue.Dequeue());

            connectionMock.Setup(o => o.Read())
                .Returns(() => readQueue.Dequeue());

            return connectionMock.Object;
        }

    }
}
