using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Entities;

namespace hMailServer.Delivery
{
    public class LocalDelivery
    {
        public LocalDelivery()
        {
            
        }

        public Task DeliverAsync(Message message)
        {
            return Task.FromResult(0);
        }
    }
}
