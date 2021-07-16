using EmailGetService.Interfaces;
using MassTransit;
using System.Threading.Tasks;
using TTDoc.EventBus.Contracts.EmailReceiver;

namespace EmailGetService.EventBus.Consumers
{
    public class MailBoxCreateConsumer : IConsumer<MailBoxCreated>
    {
        private readonly IMailBoxService mailBoxService;

        public MailBoxCreateConsumer(IMailBoxService mailBoxService)
        {
            this.mailBoxService = mailBoxService;
        }

        public async Task Consume(ConsumeContext<MailBoxCreated> context)
        {
            var mailBox = context.Message;

            if (mailBox != null)
            {
                await mailBoxService.CreateMailBox(mailBox.ImapServer, mailBox.ImapPort, mailBox.UserName, mailBox.Password, mailBox.IsActive);
            }
        }
    }
}
