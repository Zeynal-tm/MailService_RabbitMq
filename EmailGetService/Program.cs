using AutoMapper;
using EmailGetService.Database;
using EmailGetService.EventBus.Consumers;
using EmailGetService.Interfaces;
using EmailGetService.Models;
using EmailGetService.Options;
using EmailGetService.Services;
using GreenPipes;
using MassTransit;
using MassTransit.MongoDbIntegration.MessageData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using TTDoc.EventBus.Contracts.EmailSender;

namespace EmailGetService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, builder) =>
                 {
                     builder.AddEnvironmentVariables()
                            .AddJsonFile($"serilogconfig.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                            .AddJsonFile("serilogconfig.json");

                     if (context.HostingEnvironment.IsDevelopment())
                     {
                         builder.AddJsonFile($"identityconfig.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                     }
                     else
                     {
                         LoadConfigs(builder);
                     }
                 })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<EmailDbContext>(options => options.UseNpgsql(hostContext.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));

                    services.Configure<RabbitMQOptions>(hostContext.Configuration.GetSection("EventBus"));

                    services.Configure<EmailMonitoringOptions>(hostContext.Configuration.GetSection("EmailMonitoringOptions"));

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MailBoxCreateConsumer>();
                        x.AddConsumer<MailBoxUpdateConsumer>();
                        x.AddConsumer<MailBoxDeleteConsumer>();
                        x.AddConsumer<CheckEmailAuthenticationConsumer>();
                        x.AddConsumer<CheckEmailNowConsumer>();

                        var messageDataRepository = new MongoDbMessageDataRepository(hostContext.Configuration["Mongo:ConnectionString"], hostContext.Configuration["Mongo:MassTransitDatabase"]);

                        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                            {
                                EndpointConvention.Map<SendEmail>(new Uri("queue:email-sender"));
                                EndpointConvention.Map<SendEmailNow>(new Uri("queue:email-sender-to"));

                                var options = provider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

                                cfg.UseMessageData(messageDataRepository);

                                cfg.Host(options.IPAddress, "/", h =>
                                {
                                    h.Username(options.UserName);
                                    h.Password(options.Password);
                                });

                                cfg.ReceiveEndpoint("create-mailBox", ep =>
                                {
                                    ep.PrefetchCount = 16;
                                    ep.UseMessageRetry(r => r.Interval(2, 100));
                                    ep.ConfigureConsumer<MailBoxCreateConsumer>(provider);
                                });

                                cfg.ReceiveEndpoint("update-mailBox", ep =>
                                {
                                    ep.PrefetchCount = 16;
                                    ep.UseMessageRetry(r => r.Interval(2, 100));
                                    ep.ConfigureConsumer<MailBoxUpdateConsumer>(provider);
                                });

                                cfg.ReceiveEndpoint("delete-mailBox", ep =>
                                {
                                    ep.PrefetchCount = 16;
                                    ep.UseMessageRetry(r => r.Interval(2, 100));
                                    ep.ConfigureConsumer<MailBoxDeleteConsumer>(provider);
                                });

                                cfg.ReceiveEndpoint("check-email-authentication", ep =>
                                {
                                    ep.PrefetchCount = 16;
                                    ep.UseMessageRetry(r => r.Interval(2, 100));
                                    ep.ConfigureConsumer<CheckEmailAuthenticationConsumer>(provider);
                                });

                                cfg.ReceiveEndpoint("check-email-now", ep =>
                                {
                                    ep.PrefetchCount = 16;
                                    ep.UseMessageRetry(r => r.Interval(2, 100));
                                    ep.ConfigureConsumer<CheckEmailNowConsumer>(provider);
                                });
                            }));
                    });

                    services.AddMassTransitHostedService();

                    services.AddHostedService<EmailCheckTimedHostedService>();

                    services.AddHostedService<EmailSenderTimedHostedService>();

                    services.AddDataProtection();

                    services.AddScoped<IEmailReceiverService, EmailReceiverService>();

                    services.AddScoped<IMailBoxService, MailBoxService>();

                    services.AddScoped<IEmailSenderService, EmailSenderService>();

                    services.AddAutoMapper(Assembly.GetExecutingAssembly());
                })
                 .UseSerilog((context, configuration) =>
                 {
                     configuration.Enrich.FromLogContext().ReadFrom.Configuration(context.Configuration);
                 });

        private static void LoadConfigs(IConfigurationBuilder builder)
        {
            var files = Directory.GetFiles("/config", "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                builder.AddJsonFile(file);
            }
        }
    }
}
