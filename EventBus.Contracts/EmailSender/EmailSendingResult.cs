using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTDoc.EventBus.Contracts.EmailSender
{
    public interface EmailSendingResult
    {
        public bool Synchronized { get; set; }
    }
}
