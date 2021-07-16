using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmailGetService.Database;
using EmailGetService.Models;
using EmailGetService.Models.DTO;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmailGetService.Services
{
    public class EmailReceiverService : IEmailReceiverService
    {
        private readonly ILogger<EmailReceiverService> logger;
        private readonly EmailDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IDataProtector dataProtector;
        private readonly ImapClient client;

        public EmailReceiverService(ILogger<EmailReceiverService> logger, EmailDbContext dbContext, IMapper mapper, IDataProtectionProvider protectionProvider)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.mapper = mapper;
            client = new ImapClient();
            dataProtector = protectionProvider.CreateProtector("EmailPasswordProtection");
        }

        public async Task ReceiveNewEmailsAsync(MailBox mailBox)
        {
            var client = await ConnectAsync(mailBox);

            if (client.IsAuthenticated)
            {
                var deliveredAfter = await GetLastRecievedEmailDateAsync();

                var uids = await client.Inbox.SearchAsync(SearchQuery.DeliveredAfter(deliveredAfter));

                if (uids.Any())
                {
                    await SaveNewEmailsAsync(uids, mailBox);
                }
                await client.DisconnectAsync(true);
                logger.LogInformation($"Disconnected");
            }
        }

        public async Task<bool> CheckConnectionAsync(string imapServer, int port, string userName, string password)
        {
            try
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(imapServer, port);
                await client.AuthenticateAsync(userName, password);

                logger.LogInformation($"{userName} connection is checked and was successful");

                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        public async Task<ImapClient> ConnectAsync(MailBox mailBox)
        {
            try
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(mailBox.ImapServer, mailBox.ImapPort);
                await client.AuthenticateAsync(mailBox.UserName, dataProtector.Unprotect(mailBox.Password));
                await client.Inbox.OpenAsync(FolderAccess.ReadWrite);

                logger.LogInformation($"{mailBox.UserName} connection was successful");

                return client;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                return client;
            }
        }

        public async Task SaveNewEmailsAsync(IList<UniqueId> uids, MailBox mailBox)
        {
            int countOfAllNewRecievedEmails = 0;
            int countOfAllNewRecievedAttachments = 0;
            foreach (var item in uids)
            {
                var recievedMessage = await client.Inbox.GetMessageAsync(item);

                var message = await dbContext.Emails.AnyAsync(m => m.MessageId == recievedMessage.MessageId);
                if (!message)
                {
                    if (recievedMessage.Attachments != null && recievedMessage.Attachments.Any())
                    {
                        ++countOfAllNewRecievedEmails;
                        countOfAllNewRecievedAttachments += recievedMessage.Attachments.Count();
                        var email = await AddEmailAsync(recievedMessage, mailBox);
                        await AddAttachmentAsync(email, recievedMessage.Attachments);
                        client.Inbox.AddFlags(item, MessageFlags.Seen, true);
                    }
                }
            }
            await dbContext.SaveChangesAsync();
            logger.LogInformation($"Recieved {countOfAllNewRecievedEmails} emails with {countOfAllNewRecievedAttachments} attachments");
        }

        public async Task<DateTime> GetLastRecievedEmailDateAsync()
        {
            DateTime deliveredAfter = DateTime.Now.Date;

            if (await dbContext.Emails.AnyAsync())
            {
                var lastReceivedEmailDate = await dbContext.Emails.MaxAsync(p => p.RecievedDate);
                deliveredAfter = lastReceivedEmailDate.LocalDateTime;

                logger.LogInformation($"The last email recieved date is : {deliveredAfter}");
            }

            return deliveredAfter;
        }

        public async Task<Email> AddEmailAsync(MimeMessage recievedMessage, MailBox mailBox)
        {
            var email = await dbContext.AddAsync(new Email
            {
                MessageId = recievedMessage.MessageId,
                Subject = recievedMessage.Subject,
                Message = recievedMessage.TextBody,
                Sender = recievedMessage.From.Mailboxes.First().Address,
                RecievedDate = recievedMessage.Date.LocalDateTime,
                Synchronized = false,
                MailBox = mailBox
            });

            logger.LogInformation($"Recieved email from {email.Entity.Sender} with {recievedMessage.Attachments.Count()} attachments at : {email.Entity.RecievedDate}");
            return email.Entity;
        }

        public async Task AddAttachmentAsync(Email email, IEnumerable<MimeEntity> attachments)
        {
            if (attachments.Any())
            {
                int countOfAttachments = 0;
                foreach (var recievedAttachment in attachments)
                {
                    var fileName = recievedAttachment.ContentDisposition?.FileName ?? recievedAttachment.ContentType.Name;

                    using var ms = new MemoryStream();
                    await recievedAttachment.WriteToAsync(ms);

                    await dbContext.AddAsync(new Attachment
                    {
                        Email = email,
                        Name = fileName,
                        AttachmentInBytes = ms.ToArray()
                    });

                    logger.LogInformation($"{++countOfAttachments}. {fileName}");
                }
            }
        }

        public async Task<IEnumerable<EmailDto>> GetUnSynchronizedEmailsAsync(MailBox mailBox)
        {
            return await Task.FromResult(dbContext.Emails.Include(a => a.Attachments).Where(m => m.MailBoxId == mailBox.Id && !m.Synchronized).ProjectTo<EmailDto>(mapper.ConfigurationProvider));
        }
    }
}
