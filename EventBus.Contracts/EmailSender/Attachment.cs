using MassTransit;

namespace TTDoc.EventBus.Contracts.EmailSender
{
    public interface Attachment
    {
        public string Name { get; }
        public MessageData<byte[]> AttachmentInBytes { get; }
    }
}
