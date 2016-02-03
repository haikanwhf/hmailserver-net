using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSession : ISession
    {
        private IConnection _connection;
        private readonly ISmtpServerCommandHandler _commandHandler;

        public SmtpServerSession(ISmtpServerCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
        }

        public async Task HandleConnection(IConnection connection)
        {
            _connection = connection;

            await _connection.WriteString("220\r\n");

            while (true)
            {
                var data = await _connection.ReadUntilNewLine();

                data = data.ToLowerInvariant();

                if (data.StartsWith("helo"))
                {
                    var hostName = CommandParser.ParseHelo(data);

                    _commandHandler.HandleHelo(hostName);

                    await _connection.WriteString("250 HELLO\r\n");
                }
                else if (data.StartsWith("mail from"))
                {
                    var fromAddress = CommandParser.ParseMailFrom(data);
                    _commandHandler.HandleMailFrom(fromAddress);
                    await _connection.WriteString("250 OK\r\n");
                }
                else if (data.StartsWith("rcpt to"))
                {
                    var fromAddress = CommandParser.ParseRcptTo(data);
                    _commandHandler.HandleRcptTo(fromAddress);
                    await _connection.WriteString("250 OK\r\n");
                }
                else if (data.StartsWith("data"))
                {
                    await _connection.WriteString("354 OK, send\r\n");

                    var maildata = await _connection.ReadStringUntil(".\r\n");
                    var bytes = Encoding.UTF8.GetBytes(maildata);

                    var memoryStream = new MemoryStream(bytes);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    _commandHandler.HandleData(memoryStream);

                    await _connection.WriteString("250 queued!\r\n");
                }
            }
        }

    }

}
