using Microsoft.Extensions.Options;
using Photobook.Common.Configuration;
using Photobook.Notifications.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Photobook.Notifications.Client
{
    public class SmtpProvider : ISmtpProvider
    {
        private readonly SmtpOptions _config;

        public SmtpProvider(IOptions<AppOptions> options)
        {
            _config = options.Value.Smtp;
        }

        public async Task<bool> SendNotificationAsync(IEmailNotification notification)
        {
            if (notification == null)
            {
                return false;
            }

            var emailNotification = notification as EmailNotification;

            var message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(_config.FromEmail, _config.FromName),
                Subject = emailNotification.Subject,
                Body = emailNotification.ParsedResult
            };

            foreach (var emailAddress in emailNotification.To.Split(';'))
            {
                message.To.Add(new MailAddress(emailAddress));
            }

            using var client = new SmtpClient(_config.Host, _config.Port);
            client.Credentials = new NetworkCredential(_config.SmtpUsername, _config.SmtpPassword);
            client.EnableSsl = _config.EnableSsl;
            try
            {
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
