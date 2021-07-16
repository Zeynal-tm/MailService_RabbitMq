using EmailGetService.Models;
using EmailGetService.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailGetService.Services
{
    public interface IEmailReceiverService
    {
        /// <summary>
        /// Проверить подключение
        /// </summary>
        /// <param name="imapServer"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> CheckConnectionAsync(string imapServer, int port, string userName, string password);
        /// <summary>
        /// Асинхронно получать новые электронные письма с вложениями
        /// </summary>
        /// <returns></returns>
        Task ReceiveNewEmailsAsync(MailBox mail);
        /// <summary>
        /// Получить все непрочитанные письма
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EmailDto>> GetUnSynchronizedEmailsAsync(MailBox mail);
    }
}