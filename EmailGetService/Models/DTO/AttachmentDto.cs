namespace EmailGetService.Models.DTO
{
    public class AttachmentDto
    {
        /// <summary>
        /// Имя вложения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Вложение в байтах
        /// </summary>
        public byte[] AttachmentInBytes { get; set; }
    }
}
