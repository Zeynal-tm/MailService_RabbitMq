using EmailGetService.Database;
using EmailGetService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TTDoc.EventBus.Contracts.EmailReceiver;

namespace EmailGetService.EventBus.Consumers
{
    public class CheckEmailNowConsumer : IConsumer<CheckEmailNow>
    {
        public IServiceProvider Services { get; }

        public CheckEmailNowConsumer(IServiceProvider services)
        {
            Services = services;
        }

        public async Task Consume(ConsumeContext<CheckEmailNow> context)
        {
            using var scope = Services.CreateScope();

            var scopeDbContext = scope.ServiceProvider.GetRequiredService<EmailDbContext>();

            var emailBox = await scopeDbContext.MailBoxes.FirstOrDefaultAsync(m => m.UserName == context.Message.UserName);

            var scopedEmailReceiverService = scope.ServiceProvider.GetRequiredService<IEmailReceiverService>();

            await scopedEmailReceiverService.ReceiveNewEmailsAsync(emailBox);

            var scopedEmailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
            var newMessage = await scopedEmailSenderService.SendUnSynchronizedEmail(context.Message.UserName);

            await context.RespondAsync<CheckEmailNowResult>(new { NewMessage = newMessage });
        }
    }
}
