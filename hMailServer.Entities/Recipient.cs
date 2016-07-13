namespace hMailServer.Entities
{
    public class Recipient
    {
        public ulong Id { get; set; }
        public ulong MessageId { get; set; }
        public string Address { get; set; }
        public string OriginalAddress { get; set; }
        public ulong AccountId { get; set; }
    }
}
