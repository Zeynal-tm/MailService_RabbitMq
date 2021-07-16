namespace TTDoc.EventBus.Contracts.EmailReceiver
{
    public interface MailBoxCreated
    {
        public string ImapServer { get; }
        public int ImapPort { get; }
        public string UserName { get; }
        public string Password { get; }
        public bool IsActive { get; set; }
    }
}
