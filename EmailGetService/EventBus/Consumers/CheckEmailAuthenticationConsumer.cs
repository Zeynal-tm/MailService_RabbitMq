using EmailGetService.Services;
using MassTransit;
using System.Threading.Tasks;
using TTDoc.EventBus.Contracts.EmailReceiver;

namespace EmailGetService.EventBus.Consumers
{
    public class CheckEmailAuthenticationConsumer : IConsumer<CheckEmailAuthentication>
    {
        private readonly IEmailReceiverService emailService;

        public CheckEmailAuthenticationConsumer(IEmailReceiverService emailService)
        {
            this.emailService = emailService;
        }

        public async Task Consume(ConsumeContext<CheckEmailAuthentication> context)
        {
            var mailBox = context.Message;

            if (mailBox != null)
            {
                var succeeded = await emailService.CheckConnectionAsync(mailBox.ImapServer, mailBox.ImapPort, mailBox.UserName, mailBox.Password);

                await context.RespondAsync<EmailAuthenticationResult>(new { Succeeded = succeeded });
            }
        }
    }
}
