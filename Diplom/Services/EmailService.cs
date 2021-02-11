﻿using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Security;

namespace Diplom.Services
{
    public class EmailService : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "qweqw.qwe.88@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, true);

                await client.AuthenticateAsync("эмайл@mail.ru", "пароль");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
