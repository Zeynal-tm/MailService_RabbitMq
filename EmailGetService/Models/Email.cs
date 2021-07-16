using System;
using System.Collections.Generic;

namespace EmailGetService.Models
{
    /// <summary>
    /// Электронное письмо
    /// </summary>
    public class Email
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        /// <summary>
        /// Отправитель
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// Тема
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Дата получения
        /// </summary>
        public DateTimeOffset RecievedDate { get; set; }
        /// <summary>
        /// Было ли письмо прочитано
        /// </summary>
        public bool Synchronized { get; set; }
        /// <summary>
        /// Вложения
        /// </summary>
        public ICollection<Attachment> Attachments { get; set; }
        public int MailBoxId { get; set; }
        public MailBox MailBox { get; set; }
    }
}
