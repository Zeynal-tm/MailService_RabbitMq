using System;

namespace TTDoc.EventBus.Contracts.EmailSender
{
    public interface SendEmail
    {
        public string UserName { get; set; }
        public string Sender { get; }
        public string Subject { get; }
        public string Message { get; }
        public DateTimeOffset RecievedDate { get; }
        public Attachment[] Attachments { get; }
    }
}
