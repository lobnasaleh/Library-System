using LMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("librarysystem80@gmail.com", "bjnh wnop asmd gyqb"), //  app-specific password
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("librarysystem80@gmail.com", "LibraryManagementSystem"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
