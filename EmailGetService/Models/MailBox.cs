using System.Collections.Generic;

namespace EmailGetService.Models
{
    /// <summary>
    /// Почтовой ящик
    /// </summary>
    public class MailBox
    {
        public int Id { get; set; }
        public string ImapServer { get; set; }
        public int ImapPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Email> Emails { get; set; }
    }
}
