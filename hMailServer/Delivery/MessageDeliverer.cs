using System;
using System.Threading;
using System.Threading.Tasks;
using hMailServer.Entities;
using hMailServer.Repository;
using StructureMap;

namespace hMailServer.Delivery
{
    class MessageDeliverer
    {
        private readonly IContainer _container;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        public MessageDeliverer(IContainer container)
        {
            _container = container;
            _cancellationToken = _cancellationTokenSource.Token;

        }

        public async Task RunAsync()
        {
            var messageRepository = _container.GetInstance<IMessageRepository>();

            while (true)
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var message = await messageRepository.GetMessageToDeliverAsync();

                if (message != null)
                {
                    await DeliverMessageAsync(message);
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), _cancellationToken);
            }
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();

            // TODO: When what?
            await Task.WhenAll();
        }

        private Task DeliverMessageAsync(Message message)
        {
            var localDelivery = new LocalDelivery();
            return localDelivery.DeliverAsync(message);
        }
    }
}
