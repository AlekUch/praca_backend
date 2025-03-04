﻿
using MailKit.Net.Smtp;
using MimeKit;

namespace AGROCHEM.Services
{

    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            string host = smtpSettings["Host"];
            int port = int.Parse(smtpSettings["Port"]);
            bool enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
            string username = smtpSettings["Username"];
            string password = smtpSettings["Password"];
            


            var message = new MimeMessage();
                message.From.Add(new MailboxAddress("AGROCHEM", username)); 
                message.To.Add(new MailboxAddress(to, to)); 
                message.Subject = subject; 
                message.Body = new TextPart("html")
                {
                    Text = body
                };
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate(username, password);
                    client.Send(message);
                    Console.WriteLine("E-mail został wysłany pomyślnie.");
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas połączenia: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Szczegóły: {ex.InnerException.Message}");
                    }
                }
            }            
        }
    }

}