using System.Threading.Tasks;

namespace EmailGetService.Interfaces
{
    public interface IMailBoxService
    {
        /// <summary>
        /// Создать почтовый ящик
        /// </summary>
        /// <param name="imapServer"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task CreateMailBox(string imapServer, int port, string userName, string password, bool isActive);
        /// <summary>
        /// Обновить почтовый ящик
        /// </summary>
        /// <param name="imapServer"></param>
        /// <param name="port"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="oldUserName"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        Task UpdateMailBox(string imapServer, int port, string userName, string password, string oldUserName, bool isActive);
        /// <summary>
        /// Удалить почтовый ящик
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task RemoveMailBox(string userName);
    }
}
