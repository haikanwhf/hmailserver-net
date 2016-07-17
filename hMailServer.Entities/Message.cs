using System;
using System.Collections.Generic;

namespace hMailServer.Entities
{
    public class Message
    {
        public long Id { get; set; }
        public long Uid { get; set; }
        public long AccountId { get; set; }
        public long FolderId { get; set; }
        public long Size { get; set; }
        public MessageState State { get; set; }
        public short NumberOfDeliveryAttempts { get; set; }
        public short Flags { get; set; }
        public string Filename { get; set; }
        public string From { get; set; }
        public bool Locked { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime NextDeliveryAttempt { get; set; }

        public List<Recipient> Recipients { get; set; } = new List<Recipient>();

        public Message Clone()
        {
            var clone = (Message) MemberwiseClone();

            clone.Recipients = new List<Recipient>();

            foreach (var recipient in Recipients)
            {
                clone.Recipients.Add(recipient.Clone());
            }

            return clone;
        }
    }
}
