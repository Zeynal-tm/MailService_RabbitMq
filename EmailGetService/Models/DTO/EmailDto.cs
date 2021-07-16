using System;
using System.Collections.Generic;

namespace EmailGetService.Models.DTO
{
    public class EmailDto
    {
        /// <summary>
        /// Получатель
        /// </summary>
        public string Receiver { get; set; }
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
        public ICollection<AttachmentDto> Attachments { get; set; }
    }
}
