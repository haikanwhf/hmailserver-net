using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSession : ISession
    {
        private IConnection _connection;
        private readonly ISmtpServerCommandHandler _commandHandler;
        private readonly SmtpServerSessionState _state = new SmtpServerSessionState();

        public SmtpServerSession(ISmtpServerCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public async Task HandleConnection(IConnection connection)
        {
            _connection = connection;

            try
            {
                await SendBanner();

                while (true)
                {
                    var data = await ReadUntilNewLine(_connection);

                    var command = CommandParser.ParseCommand(data);

                    if (!_state.IsCommandValid(command))
                    {
                        await _connection.WriteString("503 bad sequence of commands\r\n");
                        continue;
                    }

                    switch (command)
                    {
                        case SmtpCommand.Helo:
                            await HandleHelo(data);
                            break;
                        case SmtpCommand.Ehlo:
                            await HandleEhlo(data);
                            break;
                        case SmtpCommand.MailFrom:
                            await HandleMailFrom(data);
                            break;
                        case SmtpCommand.RcptTo:
                            await HandleRcptTo(data);
                            break;
                        case SmtpCommand.Data:
                            await HandleData();
                            break;
                        case SmtpCommand.Quit:
                            await HandleQuit();
                            throw new DisconnectedException();
                    }
                }
            }
            catch (DisconnectedException)
            {
                // Connection gone.
            }
        }

        private async Task HandleQuit()
        {
            await _connection.WriteString("221 Bye\r\n");
        }

        private async Task HandleData()
        {
            await _connection.WriteString("354 OK, send\r\n");

            var target = new MemoryStream();

            var transmissionBuffer = new TransmissionBuffer(target);

            var maildata = await _connection.Read();
            _connection.SkipForwardInReadStream((int) maildata.Length);
            transmissionBuffer.Append(maildata);

            while (!transmissionBuffer.TransmissionEnded)
            {
                maildata = await _connection.Read();
                _connection.SkipForwardInReadStream((int) maildata.Length);
                transmissionBuffer.Append(maildata);
            }

            _commandHandler.HandleData(target);

            await _connection.WriteString("250 queued!\r\n");
        }

        private async Task HandleRcptTo(string data)
        {
            var rcptTo = CommandParser.ParseRcptTo(data);
            _commandHandler.HandleRcptTo(rcptTo);
            await _connection.WriteString("250 OK\r\n");

            _state.HasRcptTo = true;
        }

        private async Task HandleMailFrom(string data)
        {
            var fromAddress = CommandParser.ParseMailFrom(data);
            _commandHandler.HandleMailFrom(fromAddress);
            await _connection.WriteString("250 OK\r\n");

            _state.HasMailFrom = true;
        }

        private async Task HandleEhlo(string data)
        {
            var ehloHostName = CommandParser.ParseEhlo(data);
            _commandHandler.HandleEhlo(ehloHostName);
            await _connection.WriteString("250 HELLO\r\n");

            _state.HasHelo = true;
        }

        private async Task HandleHelo(string data)
        {
            var heloHostName = CommandParser.ParseHelo(data);
            _commandHandler.HandleHelo(heloHostName);
            await _connection.WriteString("250 HELLO\r\n");

            _state.HasHelo = true;
        }

        private Task SendBanner()
        {
            string banner = string.Format(CultureInfo.InvariantCulture, "220 {0} ESMTP", Environment.MachineName);

            return _connection.WriteString(banner);
        }

        public async Task<string> ReadUntilNewLine(IConnection connection)
        {
            return await connection.ReadStringUntil("\r\n");
        }

    }

}
