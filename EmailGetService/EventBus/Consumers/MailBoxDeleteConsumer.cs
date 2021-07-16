using EmailGetService.Interfaces;
using MassTransit;
using System.Threading.Tasks;
using TTDoc.EventBus.Contracts.EmailReceiver;

namespace EmailGetService.EventBus.Consumers
{
    public class MailBoxDeleteConsumer : IConsumer<MailBoxDeleted>
    {
        private readonly IMailBoxService mailBoxService;

        public MailBoxDeleteConsumer(IMailBoxService mailBoxService)
        {
            this.mailBoxService = mailBoxService;
        }

        public async Task Consume(ConsumeContext<MailBoxDeleted> context)
        {
            var mailBox = context.Message;

            if (mailBox != null)
            {
                await mailBoxService.RemoveMailBox(mailBox.UserName);
            }
        }
    }
}
