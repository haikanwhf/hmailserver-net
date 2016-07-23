using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hMailServer.Core;
using hMailServer.Core.Dns;
using hMailServer.Core.Entities;
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
            var folderRepository = _container.GetInstance<IFolderRepository>();
            var dnsClient = _container.GetInstance<IDnsClient>();

            message.NumberOfDeliveryAttempts++;

            bool isLastAttempt = message.NumberOfDeliveryAttempts >= 3;

            var deliveryResults = new List<DeliveryResult>();

            try
            {
                var remainingRecipients = new List<Recipient>(message.Recipients);

                var localDelivery = new LocalDelivery(accountRepository, messageRepository, folderRepository, _log);
                deliveryResults.AddRange(await localDelivery.DeliverAsync(message, remainingRecipients.Where(recipient => recipient.AccountId != 0).ToList()));

                var externalDelivery = new ExternalDelivery(messageRepository, dnsClient, _log);
                deliveryResults.AddRange(await externalDelivery.DeliverAsync(message, remainingRecipients.Where(recipient => recipient.AccountId == 0).ToList()));

                var failedRecipients =
                    deliveryResults.Where(result => result.ReplyCodeSeverity == ReplyCodeSeverity.PermanentNegative ||
                                                    (isLastAttempt && result.ReplyCodeSeverity == ReplyCodeSeverity.TransientNegative));

                await SubmitBounceMessageAsync(message, failedRecipients);

                var deliveryCompleted =
                    deliveryResults.Any(result => result.ReplyCodeSeverity == ReplyCodeSeverity.TransientNegative);

                if (isLastAttempt  || !deliveryCompleted)
                {
                    await messageRepository.DeleteAsync(message);
                }
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

        private async Task SubmitBounceMessageAsync(Message message, IEnumerable<DeliveryResult> failedRecipients)
        {
            if (string.IsNullOrWhiteSpace(message.From))
                return;

            if (IsMailerDaemonAddress(message.From))
                return;

            // TODO: Dont' hardcode this.
            string bounceMessage = string.Format(@"Your message did not reach some or all of the intended recipients.

   Sent: %MACRO_SENT%
   Subject: %MACRO_SUBJECT%

The following recipient(s)could not be reached:

%MACRO_RECIPIENTS%

hMailServer");

            throw new NotImplementedException();

        }

        private bool IsMailerDaemonAddress(string address)
        {
            var mailbox = EmailAddressParser.GetMailbox(address);

            return mailbox.Equals("MAILER-DAEMON", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
