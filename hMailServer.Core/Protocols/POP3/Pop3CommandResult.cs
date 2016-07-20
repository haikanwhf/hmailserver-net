using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.POP3
{
    public class Pop3CommandResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public Pop3CommandResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static Pop3CommandResult CreateDefaultSuccess()
        {
            return new Pop3CommandResult(true, "OK");
        }

        public static Task<Pop3CommandResult> CreateDefaultSuccessTask()
        {
            return Task.Run(() => CreateDefaultSuccess());
        }
    }
}
