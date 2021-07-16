namespace TTDoc.EventBus.Contracts.EmailReceiver
{
    public interface MailBoxUpdated
    {
        public string ImapServer { get; }
        public int ImapPort { get; }
        public string UserName { get;  }
        public string Password { get; }
        public string OldUserName { get;  }
        public bool IsActive { get; set; }
    }
}
