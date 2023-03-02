using System;
using System.Net;
using System.Net.Mail;

namespace ShopApp.WebUI.EmailServices
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendEmailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendEmailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new();
            mail.IsBodyHtml = isBodyHtml;
            foreach (var to in tos)
            {
                mail.To.Add(to);
            };
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new (_configuration["EmailSender:UserName"], "ShopApp", System.Text.Encoding.UTF8);

            SmtpClient smtp = new();
            smtp.Credentials = new NetworkCredential(_configuration["EmailSender:UserName"], _configuration["EmailSender:Password"]);
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Host = _configuration["EmailSender:Host"];

            await smtp.SendMailAsync(mail);
         }
    }
}

