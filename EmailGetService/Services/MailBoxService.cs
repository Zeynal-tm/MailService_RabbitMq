using EmailGetService.Database;
using EmailGetService.Interfaces;
using EmailGetService.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EmailGetService.Services
{
    public class MailBoxService : IMailBoxService
    {
        private readonly ILogger<MailBoxService> logger;
        private readonly EmailDbContext dbContext;
        private readonly IDataProtector dataProtector;

        public MailBoxService(ILogger<MailBoxService> logger, EmailDbContext dbContext, IDataProtectionProvider protectionProvider)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            dataProtector = protectionProvider.CreateProtector("EmailPasswordProtection");
        }

        public async Task CreateMailBox(string imapServer, int port, string userName, string password, bool isActive)
        {
            dbContext.MailBoxes.Add(new MailBox
            {
                ImapServer = imapServer,
                ImapPort = port,
                UserName = userName,
                Password = dataProtector.Protect(password),
                IsActive = isActive,
            });
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"Created new {userName} mailbox");
        }

        public async Task UpdateMailBox(string imapServer, int port, string userName, string password, string oldUserName, bool isActive)
        {
            var mailBox = await dbContext.MailBoxes.FirstOrDefaultAsync(e => e.UserName == oldUserName);

            if (mailBox != null)
            {
                mailBox = new MailBox
                {
                    ImapServer = imapServer,
                    ImapPort = port,
                    UserName = userName,
                    Password = dataProtector.Protect(password),
                    IsActive = isActive
                };

                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Updated password of {userName} mailbox");
            }
        }

        public async Task RemoveMailBox(string userName)
        {
            var mailBox = await dbContext.MailBoxes.FirstOrDefaultAsync(e => e.UserName == userName);

            if (mailBox != null)
            {
                dbContext.Remove(mailBox);

                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Deleted {userName} mailbox");
            }
        }
    }
}
