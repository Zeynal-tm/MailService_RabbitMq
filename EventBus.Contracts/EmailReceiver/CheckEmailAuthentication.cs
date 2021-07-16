namespace TTDoc.EventBus.Contracts.EmailReceiver
{
    public interface CheckEmailAuthentication
    {
        public string ImapServer { get; }
        public int ImapPort { get; }
        public string UserName { get; }
        public string Password { get; }
    }
}
