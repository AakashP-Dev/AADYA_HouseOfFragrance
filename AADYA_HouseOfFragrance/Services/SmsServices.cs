using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AADYA_HouseOfFragrance.Services
{
    public class SmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromNumber = _configuration["Twilio:FromNumber"];

            // If Twilio credentials are missing, just log (for development)
            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
            {
                Console.WriteLine($"[SMS] To: {toPhoneNumber}, Message: {message}");
                return;
            }

            try
            {
                TwilioClient.Init(accountSid, authToken);

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );

                Console.WriteLine($"SMS sent: {messageResource.Sid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMS failed: {ex.Message}");
                // Don't throw – we don't want to break the flow if SMS fails
            }
        }

        public string GetOtpSmsBody(string otp, string userName)
        {
            return $"AADYA: Your password reset OTP is {otp}. Valid for 10 minutes. Do not share this with anyone.";
        }
    }
}