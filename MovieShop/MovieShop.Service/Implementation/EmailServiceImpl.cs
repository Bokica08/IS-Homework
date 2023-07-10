using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using MovieShop.Domain.Domain;
using MovieShop.Domain;
using MovieShop.Service.Implementation;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace MovieShop.Service.Interface
{
    public class EmailServiceImpl : IEmailService
    {
        private readonly EmailSettings _settings;
        public EmailServiceImpl(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;   
        }

        public async Task SendEmailAsync(List<EmailMessage> allMails)
        {
            List<MimeMessage> messages = new List<MimeMessage>();

            foreach (var item in allMails)
            {
                var emailMessage = new MimeMessage
                {
                    Sender = new MailboxAddress(_settings.SendersName, _settings.SmtpUserName),
                    Subject = item.Subject
                };

                emailMessage.From.Add(new MailboxAddress(_settings.EmailDisplayName, _settings.SmtpUserName));

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = item.Content };

                emailMessage.To.Add(new MailboxAddress(item.MailTo, item.MailTo));

                messages.Add(emailMessage);
            }

            try
            {
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    var socketOptions = _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                    await smtp.ConnectAsync(_settings.SmtpServer, _settings.SmtpServerPort, socketOptions);

                    if (!string.IsNullOrEmpty(_settings.SmtpUserName))
                    {
                        await smtp.AuthenticateAsync(_settings.SmtpUserName, _settings.SmtpPassword);
                    }

                    foreach (var item in messages)
                    {
                        await smtp.SendAsync(item);
                    }

                    await smtp.DisconnectAsync(true);
                }
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
        }
    }
}
