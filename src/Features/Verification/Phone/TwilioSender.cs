// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Phone
{
    using Apollo.Settings;
    using Twilio.Exceptions;
    using Twilio.Rest.Api.V2010.Account;
    using Twilio.Types;

    public class TwilioSender
    {
        private readonly AppSettings appSettings;

        public TwilioSender(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public void Send(string phoneNumber, VerificationCode code)
        {
            try
            {
                MessageResource.Create(
                    body: code.ToString(),
                    from: new PhoneNumber(this.appSettings.Twilio.FromPhoneNumber),
                    to: new PhoneNumber(phoneNumber));
            }
            catch (ApiException e) when (e.Code == 21211 || e.Code == 21612 || e.Code == 21408 || e.Code == 21610 || e.Code == 21614)
            {
                // https://www.twilio.com/docs/iam/test-credentials#test-sms-messages-parameters-To
                throw new SenderException("Error sending SMS message", e);
            }
        }
    }
}
