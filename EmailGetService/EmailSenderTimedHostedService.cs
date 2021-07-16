using EmailGetService.Database;
using EmailGetService.Models;
using EmailGetService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailGetService
{
    public class EmailSenderTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<EmailSenderTimedHostedService> logger;
        private Timer timer;
        public IServiceProvider Services { get; }
        EmailMonitoringOptions timeSpanOtions;

        public EmailSenderTimedHostedService(ILogger<EmailSenderTimedHostedService> logger, IServiceProvider services, IOptions<EmailMonitoringOptions> options)
        {
            this.logger = logger;
            Services = services;
            timeSpanOtions = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Email Sender Timed Hosted Service is running.");

            timer = new Timer(CheckEmail, null, TimeSpan.Zero, TimeSpan.FromMinutes(timeSpanOtions.SendEmailIntervalInMinutes));

            return Task.CompletedTask;
        }

        private async void CheckEmail(object state)
        {
            logger.LogInformation("Email Sender verification procedure");

            using var scope = Services.CreateScope();

            var scopedMailService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
            await scopedMailService.SendUnSynchronizedEmail();
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
