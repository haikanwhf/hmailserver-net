using System;
using System.Threading;
using System.Threading.Tasks;
using hMailServer.Core;
using hMailServer.Core.Logging;
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

        private ILog _log;

        public MessageDeliverer(IContainer container)
        {
            _container = container;
            _cancellationToken = _cancellationTokenSource.Token;

            _log = _container.GetInstance<ILog>();

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
                    try
                    {
                        await DeliverMessageAsync(message);
                    }
                    catch (Exception ex)
                    {
                        var logEvent = new LogEvent()
                            {
                                EventType = LogEventType.Application,
                                LogLevel = LogLevel.Error,
                                Message = ex.Message,
                                Protocol = "SMTPD",
                            };

                        _log.LogException(logEvent, ex);
                    }
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

        private async Task DeliverMessageAsync(Message message)
        {
            var accountRepository = _container.GetInstance<IAccountRepository>();
            var messageRepository = _container.GetInstance<IMessageRepository>();

            message.NumberOfDeliveryAttempts++;

            bool isLastAttempt = message.NumberOfDeliveryAttempts >= 3;

            try
            {
                var localDelivery = new LocalDelivery(accountRepository, messageRepository);

                await localDelivery.DeliverAsync(message);

                await messageRepository.DeleteAsync(message);
            }
            catch (Exception ex)
            {
                var logEvent = new LogEvent()
                    {
                        EventType = LogEventType.Application,
                        LogLevel = LogLevel.Error,
                        Protocol = "SMTPD",
                    };

                if (isLastAttempt)
                    logEvent.Message = "Failed delivering message due to an error. Giving up.";
                else
                    logEvent.Message = "Failed delivering message due to an error. Will retry later.";

                _log.LogException(logEvent, ex);

                if (isLastAttempt)
                {
                    await messageRepository.DeleteAsync(message);
                }
                else
                {
                    await messageRepository.UpdateAsync(message);
                }

            }
        }
    }
}
