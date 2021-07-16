namespace EmailGetService.Models
{
    /// <summary>
    /// Вложения
    /// </summary>
    public class Attachment
    {
        public int Id { get; set; }
        /// <summary>
        /// Имя вложения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Вложение в байтах
        /// </summary>
        public byte[] AttachmentInBytes { get; set; }
        public int EmailId { get; set; }
        /// <summary>
        /// электронная почта
        /// </summary>
        public Email Email { get; set; }
    }
}
