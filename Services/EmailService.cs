using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace HRIS_API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOtp(string toEmail, string subject, string otp)
        {
            var smtp = _config.GetSection("Smtp");

            var message = new MailMessage
            {
                From = new MailAddress(
                    smtp["FromEmail"],
                    smtp["FromName"]
                ),
                Subject = subject,
                Body = $"Your OTP Code is: {otp}",
                IsBodyHtml = false
            };

            message.To.Add(toEmail);

            var client = new SmtpClient
            {
                Host = smtp["Host"],
                Port = int.Parse(smtp["Port"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    smtp["Username"],
                    smtp["Password"]
                ),
                Timeout = 20000
            };

            client.Send(message);
        }
    }
}
