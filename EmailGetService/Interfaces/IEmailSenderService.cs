using System.Threading.Tasks;

namespace EmailGetService.Services
{
    public interface IEmailSenderService
    {
        Task SendUnSynchronizedEmail();
        Task<bool> SendUnSynchronizedEmail(string userName);
    }
}