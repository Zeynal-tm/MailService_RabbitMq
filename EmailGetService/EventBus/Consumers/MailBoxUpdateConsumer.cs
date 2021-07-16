using EmailGetService.Interfaces;
using MassTransit;
using System.Threading.Tasks;
using TTDoc.EventBus.Contracts.EmailReceiver;

namespace EmailGetService.EventBus.Consumers
{
    public class MailBoxUpdateConsumer : IConsumer<MailBoxUpdated>
    {
        private readonly IMailBoxService mailBoxService;

        public MailBoxUpdateConsumer(IMailBoxService mailBoxService)
        {
            this.mailBoxService = mailBoxService;
        }

        public async Task Consume(ConsumeContext<MailBoxUpdated> context)
        {
            var mailBox = context.Message;

            if (mailBox != null)
            {
                await mailBoxService.UpdateMailBox(mailBox.ImapServer, mailBox.ImapPort, mailBox.UserName, mailBox.Password, mailBox.OldUserName, mailBox.IsActive);
            }
        }
    }
}
