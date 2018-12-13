// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Email
{
    using System.Net.Mail;
    using Apollo.Settings;

    public class MailSender
    {
        private readonly AppSettings appSettings;

        public MailSender(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public void SendConfirmationCode(string toAddress, VerificationCode code)
        {
            using (var message = new MailMessage("test@example.com", toAddress)
            {
                Body = code.ToString(),
                Subject = "Your Lykke Confirmation Code",
            })
            {
                this.SendMailMessage(message);
            }
        }

        private void SendMailMessage(MailMessage message)
        {
            using (var client = new SmtpClient(
                this.appSettings.Smtp.Host,
                this.appSettings.Smtp.Port))
            {
                client.Send(message);
            }
        }
    }
}
