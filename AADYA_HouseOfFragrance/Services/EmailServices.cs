using System.Net;
using System.Net.Mail;

namespace AADYA_HouseOfFragrance.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";
            var portStr = smtpSettings["Port"] ?? "587";
            int.TryParse(portStr, out int port);
            var username = smtpSettings["Username"] ?? "";
            var password = smtpSettings["Password"] ?? "";
            var fromEmail = smtpSettings["FromEmail"] ?? "noreply@aadya.in";

            var smtpClient = new SmtpClient(host)
            {
                Port = port > 0 ? port : 587,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "AADYA: The House of Fragrance"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public string GetOtpEmailBody(string otp, string userName)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Montserrat', sans-serif; color: #222222; }}
                        .container {{ max-width: 500px; margin: 0 auto; padding: 2rem; background: #FAF6EF; border: 1px solid #D3C5B2; border-radius: 4px; }}
                        .header {{ text-align: center; margin-bottom: 1.5rem; }}
                        .header h1 {{ font-family: 'Playfair Display', serif; color: #3B0E14; margin: 0; }}
                        .subtitle {{ color: #666666; font-size: 0.85rem; }}
                        .otp-box {{ 
                            background: #FFFFFF; 
                            border: 2px dashed #CBA463; 
                            border-radius: 4px; 
                            padding: 1rem; 
                            text-align: center; 
                            font-size: 2rem; 
                            font-weight: 700; 
                            color: #3B0E14; 
                            letter-spacing: 4px; 
                            margin: 1rem 0; 
                        }}
                        .footer {{ margin-top: 1.5rem; padding-top: 1rem; border-top: 1px solid #D3C5B2; font-size: 0.75rem; color: #666666; text-align: center; }}
                        .expiry {{ color: #d32f2f; font-size: 0.8rem; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset OTP</h1>
                            <p class='subtitle'>AADYA: The House of Fragrance</p>
                        </div>
                        <p>Hello <strong>{userName}</strong>,</p>
                        <p>Use the OTP below to reset your password:</p>
                        <div class='otp-box'>{otp}</div>
                        <p class='expiry'>⏳ This OTP will expire in <strong>10 minutes</strong>.</p>
                        <p>If you didn't request this, please ignore this email.</p>
                        <div class='footer'>
                            &copy; 2026 AADYA: The House of Fragrance. All Rights Reserved.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}