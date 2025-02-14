
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
                     client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    // Połączenie z serwerem SMTP
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Uwierzytelnianie
                    client.Authenticate(username, password);

                    // Wysłanie wiadomości
                    client.Send(message);
                    Console.WriteLine("E-mail został wysłany pomyślnie.");

                    // Rozłączenie z serwerem
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas połączenia: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Szczegóły wewnętrzne: {ex.InnerException.Message}");
                    }
                }

            }
            
        }
    }

}