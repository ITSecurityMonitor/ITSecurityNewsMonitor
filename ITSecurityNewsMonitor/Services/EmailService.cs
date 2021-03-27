using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string address, string subject, string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_config.GetValue<string>("Email:Smtp:User"));
            email.To.Add(MailboxAddress.Parse(address));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = message };
            email.Body = builder.ToMessageBody();

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetValue<string>("Email:Smtp:Host"), _config.GetValue<int>("Email:Smtp:Port"), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetValue<string>("Email:Smtp:User"), _config.GetValue<string>("Email:Smtp:Pass"));
            smtp.Send(email);
            smtp.Disconnect(true);

            return Task.Delay(0); // dummy task to satisfy interace
        }
    }
}
