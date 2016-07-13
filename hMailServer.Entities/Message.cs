using System;
using System.Collections.Generic;

namespace hMailServer.Entities
{
    public class Message
    {
        public ulong Id { get; set; }
        public ulong Uid { get; set; }
        public ulong AccountId { get; set; }
        public ulong FolderId { get; set; }
        public ulong Size { get; set; }
        public MessageState State { get; set; }
        public short NumberOfRetries { get; set; }
        public short Flags { get; set; }
        public string Filename { get; set; }
        public string From { get; set; }
        public bool Locked { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime NextDeliveryAttempt { get; set; }
    }
}
