using EmailGetService.Database;
using EmailGetService.Models;
using EmailGetService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EmailGetService
{
    public class EmailCheckTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<EmailCheckTimedHostedService> logger;
        private Timer timer;
        public IServiceProvider Services { get; }
        EmailMonitoringOptions timeSpanOtions;

        public EmailCheckTimedHostedService(ILogger<EmailCheckTimedHostedService> logger, IServiceProvider services, IOptions<EmailMonitoringOptions> options)
        {
            this.logger = logger;
            Services = services;
            timeSpanOtions = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Email Check Timed Hosted Service is running.");

            timer = new Timer(CheckEmail, null, TimeSpan.Zero, TimeSpan.FromMinutes(timeSpanOtions.ReceiveEmailIntervalInMinutes));

            return Task.CompletedTask;
        }

        private async void CheckEmail(object state)
        {
            logger.LogInformation("Email verification procedure");

            using var scope = Services.CreateScope();

            var scopeDbContext = scope.ServiceProvider.GetRequiredService<EmailDbContext>();

            var emailBoxes = await scopeDbContext.MailBoxes.Where(a => a.IsActive).ToListAsync();

            foreach (var emailBox in emailBoxes)
            {
                var scopedMailService = scope.ServiceProvider.GetRequiredService<IEmailReceiverService>();
                await scopedMailService.ReceiveNewEmailsAsync(emailBox);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Hosted Service is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
