using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Logging;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSession : ISession
    {
        private IConnection _connection;
        private readonly ISmtpServerCommandHandler _commandHandler;
        private readonly SmtpServerSessionState _state = new SmtpServerSessionState();
        private readonly SmtpServerSessionConfiguration _configuration;
        private const int DataTransferMemoryBufferMaxSize = 1024*30;
        private readonly ILog _log;

        public SmtpServerSession(ISmtpServerCommandHandler commandHandler, ILog log, SmtpServerSessionConfiguration configuration)
        {
            _commandHandler = commandHandler;
            _configuration = configuration;
            _log = log;
        }

        public Protocol Protocol => Protocol.SMTPD;

        public async Task HandleConnection(IConnection connection)
        {
            _connection = connection;

            try
            {
                await SendBanner();

                while (true)
                {
                    _connection.SetTimeout(_configuration.EnvelopeCommandTimeout);

                    var data = await ReadUntilNewLine(_connection);

                    _log.LogInfo(new LogEvent()
                        {
                            Message = data,
                            RemoteEndpoint = _connection.RemoteEndpoint,
                            SessionId = _connection.SessionId
                        });

                    var command = CommandParser.ParseCommand(data);

                    if (!_state.IsCommandValid(command))
                    {
                        await SendCommandResult(new SmtpCommandResult(503, "bad sequence of commands"));
                        continue;
                    }

                    switch (command)
                    {
                        case SmtpCommand.Help:
                            await HandleHelp();
                            break;
                        case SmtpCommand.Rset:
                            await HandleRset();
                            _state.HasMailFrom = false;
                            _state.HasRcptTo = false;
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
                        case SmtpCommand.Mail:
                            await HandleMailFrom(data);
                            break;
                        case SmtpCommand.Rcpt:
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

        private async Task HandleHelp()
        {
            await SendCommandResult(new SmtpCommandResult(211, "211 DATA HELO EHLO MAIL NOOP QUIT RCPT RSET SAML TURN VRFY"));
        }

        private async Task HandleStartTls()
        {
            await SendCommandResult(new SmtpCommandResult(220, "Go ahead"));

            await _connection.SslHandshakeAsServer(_configuration.SslCertificate);

            await _commandHandler.HandleRset();

            _state.Reset();
        }

        private async Task HandleRset()
        {
            var commandResult = await _commandHandler.HandleRset();
            await SendCommandResult(commandResult);
        }

        private async Task HandleQuit()
        {
            await SendCommandResult(new SmtpCommandResult(221, "Bye"));
        }

        private async Task HandleData()
        {
            await SendLine("354 OK, send");

            _connection.SetTimeout(_configuration.DataCommandTimeout);

            using (var target = new MemoryStreamWithFileBacking(DataTransferMemoryBufferMaxSize, _configuration.TempDirectory))
            {
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

                var commandResult = await _commandHandler.HandleData(target);

                await SendCommandResult(commandResult);

                _state.HasMailFrom = false;
                _state.HasRcptTo = false;
            }
        }

        private async Task HandleRcptTo(string data)
        {
            var rcptTo = CommandParser.ParseRcptTo(data);
            var commandResult = await _commandHandler.HandleRcptTo(rcptTo);
            await SendCommandResult(commandResult);

            if (commandResult.IsPositive())
                _state.HasRcptTo = true;
        }

        private async Task HandleMailFrom(string data)
        {
            var fromAddress = CommandParser.ParseMailFrom(data);
            // BREAKING: Option Allow Mail From Null removed.

            if (!EmailAddressParser.IsValidEmailAddress(fromAddress))
            {
                await SendCommandResult(new SmtpCommandResult(550, "The address is not valid"));
                return;
            }
            
            var commandResult = await _commandHandler.HandleMailFrom(fromAddress);

            await SendCommandResult(commandResult);
            
            if (commandResult.IsPositive())
                _state.HasMailFrom = true;
        }

        private async Task HandleEhlo(string data)
        {
            var ehloHostName = CommandParser.ParseEhlo(data);
            var commandResult = await _commandHandler.HandleEhlo(ehloHostName);

            if (commandResult.IsPositive())
            {
                var response = new StringBuilder();
                response.AppendFormat("250-{0}\r\n", Environment.MachineName);
                response.AppendFormat("250-SIZE\r\n");
                response.AppendFormat("250-AUTH LOGIN\r\n");

                if (_configuration.SslCertificate != null)
                    response.AppendFormat("250-STARTTLS\r\n");

                response.AppendFormat("250 HELP");

                await SendLine(response.ToString());

                _state.HasHelo = true;
            }
            else
            {
                await SendCommandResult(commandResult);
            }
        }

        private async Task HandleHelo(string data)
        {
            var heloHostName = CommandParser.ParseHeloEhlo(data);
            var commandResult = await _commandHandler.HandleHelo(heloHostName);

            await SendCommandResult(commandResult);

            if (commandResult.IsPositive())
                _state.HasHelo = true;
        }

        private Task SendBanner()
        {
            string banner = string.Format(CultureInfo.InvariantCulture, "220 {0} ESMTP", _configuration.HostName);

            return SendLine(banner);
        }

        public Task<string> ReadUntilNewLine(IConnection connection)
        {
            return connection.ReadStringUntil("\r\n");
        }

        private async Task SendCommandResult(SmtpCommandResult commandResult)
        {
            if (commandResult == null)
                throw new ArgumentException("commandResult");
            
            var message = $"{commandResult.Code} {commandResult.Message}";
            await SendLine(message);
        }

        private Task SendLine(string message)
        {
            _log.LogInfo(new LogEvent()
            {
                Message = message,
                RemoteEndpoint = _connection.RemoteEndpoint,
                SessionId = _connection.SessionId
            });

            return _connection.WriteString(message + "\r\n");
        }

        
    }

}
