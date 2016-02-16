using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSession : ISession
    {
        private IConnection _connection;
        private readonly ISmtpServerCommandHandler _commandHandler;
        private readonly SmtpServerSessionState _state = new SmtpServerSessionState();
        private readonly SmtpServerSessionConfiguration _configuration;
        private const int DataTransferMemoryBufferMaxSize = 1024*30;

        public SmtpServerSession(ISmtpServerCommandHandler commandHandler, SmtpServerSessionConfiguration configuration)
        {
            _commandHandler = commandHandler;
            _configuration = configuration;
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
                        case SmtpCommand.Rset:
                            await HandleRset();
                            break;
                        case SmtpCommand.Helo:
                            await HandleHelo(data);
                            break;
                        case SmtpCommand.Ehlo:
                            await HandleEhlo(data);
                            break;
                        case SmtpCommand.StartTls:
                            await HandleStartTls();
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

        private async Task HandleStartTls()
        {
            await _connection.WriteString("220 Go ahead\r\n");

            await _connection.SslHandshakeAsServer(_configuration.SslCertificate);

            _commandHandler.HandleRset();

            _state.Reset();
        }

        private async Task HandleRset()
        {
            var commandResult = _commandHandler.HandleRset();
            await SendCommandResult(commandResult);
        }

        private async Task HandleQuit()
        {
            await _connection.WriteString("221 Bye\r\n");
        }

        private async Task HandleData()
        {
            await _connection.WriteString("354 OK, send\r\n");


            var target = new MemoryStreamWithFileBacking(DataTransferMemoryBufferMaxSize);

            var transmissionBuffer = new TransmissionBuffer(target);
            
            while (!transmissionBuffer.TransmissionEnded)
            {
                using (var maildata = await _connection.Read())
                {
                    maildata.Seek(0, SeekOrigin.Begin);
                    
                    transmissionBuffer.Append(maildata);
                }
            }

            transmissionBuffer.Flush();

            var commandResult = _commandHandler.HandleData(target);

            await SendCommandResult(commandResult);
        }

        private async Task HandleRcptTo(string data)
        {
            var rcptTo = CommandParser.ParseRcptTo(data);
            var commandResult = _commandHandler.HandleRcptTo(rcptTo);
            await SendCommandResult(commandResult);

            if (commandResult.IsPositive())
                _state.HasRcptTo = true;
        }

        private async Task HandleMailFrom(string data)
        {
            var fromAddress = CommandParser.ParseMailFrom(data);
            var commandResult = _commandHandler.HandleMailFrom(fromAddress);

            await SendCommandResult(commandResult);
            
            if (commandResult.IsPositive())
                _state.HasMailFrom = true;
        }

        private async Task HandleEhlo(string data)
        {
            var ehloHostName = CommandParser.ParseEhlo(data);
            var commandResult = _commandHandler.HandleEhlo(ehloHostName);

            if (commandResult.IsPositive())
            {
                var response = new StringBuilder();
                response.AppendFormat("250-{0}\r\n", Environment.MachineName);

                if (_configuration.SslCertificate != null)
                    response.AppendFormat("250-STARTTLS\r\n");

                response.AppendFormat("250 HELP\r\n");

                await _connection.WriteString(response.ToString());

                _state.HasHelo = true;
            }
            else
            {
                await SendCommandResult(commandResult);
            }
        }

        private async Task HandleHelo(string data)
        {
            var heloHostName = CommandParser.ParseHelo(data);
            var commandResult = _commandHandler.HandleHelo(heloHostName);

            await SendCommandResult(commandResult);

            if (commandResult.IsPositive())
                _state.HasHelo = true;
        }

        private Task SendBanner()
        {
            string banner = string.Format(CultureInfo.InvariantCulture, "220 {0} ESMTP\r\n", Environment.MachineName);

            return _connection.WriteString(banner);
        }

        public async Task<string> ReadUntilNewLine(IConnection connection)
        {
            return await connection.ReadStringUntil("\r\n");
        }

        private async Task SendCommandResult(SmtpCommandResult commandResult)
        {
            if (commandResult == null)
                throw new ArgumentException("commandResult");
            
            var message = $"{commandResult.Code} {commandResult.Message}\r\n";
            await _connection.WriteString(message);
        }


    }

}
