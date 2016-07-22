using hMailServer.Entities;

namespace hMailServer.Delivery
{
    public class DeliveryResult
    {
        public Recipient Recipient { get;  }  
        public bool Success { get; }
        public string ResultMessage { get;  }

        public DeliveryResult(Recipient recipient, bool success, string resultMessage)
        {
            Recipient = recipient;
            Success = success;
            ResultMessage = resultMessage;
        }
    }
}
