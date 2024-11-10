using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace api.Services.MailServices
{
    public class MailService : IMailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly HttpClient _httpClient;

        public MailService(EmailConfiguration emailConfiguration, HttpClient httpClient)
        {
            _emailConfiguration = emailConfiguration;
            _httpClient = httpClient;
        }

       

        public async Task<bool> IsValidEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"https://api.zerobounce.net/v2/validate?api_key={_emailConfiguration.ApiKey}&email={email}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            var status = json["status"].ToString();
            return status == "valid";
        }

        public void SendEmail(Messages message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Messages messages)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfiguration.From));
            emailMessage.To.AddRange(messages.To);
            emailMessage.Subject = messages.Subject;

            // Kiểm tra nếu messages.Content là null và gán giá trị mặc định
            var emailContent = messages.Content ?? "Nội dung email không được để trống";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = emailContent
            };
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);
                client.Send(mailMessage);

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to send email.", ex);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
