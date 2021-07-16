using EmailGetService.Database;
using EmailGetService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using TTDoc.EventBus.Contracts.EmailSender;

namespace EmailGetService.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly EmailDbContext dbContext;
        private readonly IBus bus;
        private readonly ILogger logger;

        public EmailSenderService(EmailDbContext dbContext, IBus bus, ILogger logger)
        {
            this.dbContext = dbContext;
            this.bus = bus;
            this.logger = logger;
        }

        public async Task SendUnSynchronizedEmail()
        {
            var emails = await dbContext.Emails.Include(m => m.MailBox).Include(m => m.Attachments).Where(m => m.Synchronized == false).ToListAsync();

            if (emails.Count > 0)
            {
                foreach (var email in emails)
                {
                    await SendAsync(email);
                }
            }
        }

        public async Task<bool> SendUnSynchronizedEmail(string userName)
        {
            var emails = await dbContext.Emails.Include(m => m.MailBox).Include(m => m.Attachments).Where(m => m.Synchronized == false && m.MailBox.UserName == userName).ToListAsync();

            if (emails.Count > 0)
            {
                foreach (var email in emails)
                {
                    await SendAsync(email);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task SendAsync(Email email)
        {
            var result = await bus.Request<SendEmailNow, EmailSendingResult>
                      (new
                      {

                          email.MailBox.UserName,
                          email.Sender,
                          email.Subject,
                          email.Message,
                          email.RecievedDate,
                          email.Attachments
                      });
            logger.Information($"{email.MailBox.UserName} mail is sent.");

            if (result.Message.Synchronized)
            {
                email.Synchronized = true;
                await dbContext.SaveChangesAsync();
                logger.Information($"{email.MailBox.UserName} is Synchronized and Saved to database.");
            }
        }
    }
}
