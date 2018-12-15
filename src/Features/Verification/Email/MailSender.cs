// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Email
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Apollo.Settings;
    using Microsoft.Extensions.Logging;

    public class MailSender
    {
        private readonly AppSettings appSettings;

        private readonly ILogger<MailSender> logger;

        public MailSender(AppSettings appSettings, ILogger<MailSender> logger)
        {
            this.appSettings = appSettings;
            this.logger = logger;
        }

        public async Task SendConfirmationCode(string toAddress, VerificationCode code)
        {
            using (var message = new MailMessage("joe.stead@lykke.com", toAddress) // todo configure from address
            {
                Body = code.ToString(),
                Subject = "Your Lykke Confirmation Code",
            })
            {
                await this.SendMailMessage(message);
            }
        }

        private async Task SendMailMessage(MailMessage message)
        {
            try
            {
                using (var client = new SmtpClient(
                    this.appSettings.Smtp.Host,
                    this.appSettings.Smtp.Port)
                {
                    EnableSsl = this.appSettings.Smtp.EnableSSL
                })
                {
                    if (!string.IsNullOrWhiteSpace(this.appSettings.Smtp.Username))
                    {
                        this.logger.LogDebug("Using smtp credentials");
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(this.appSettings.Smtp.Username, this.appSettings.Smtp.Password);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    }

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Could not send verification email");
                throw;
            }
        }
    }
}
