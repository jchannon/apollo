// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification.Phone
{
    using System;
    using Apollo.Settings;
    using Microsoft.Extensions.Logging;
    using Twilio.Exceptions;
    using Twilio.Rest.Api.V2010.Account;
    using Twilio.Types;

    public class TwilioSender
    {
        private readonly AppSettings appSettings;

        private readonly ILogger<TwilioSender> logger;

        public TwilioSender(AppSettings appSettings, ILogger<TwilioSender> logger)
        {
            this.appSettings = appSettings;
            this.logger = logger;
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
                this.logger.LogWarning("User tried to use an invalid phone number ({phoneNumber}) to receive a confirmation code", phoneNumber);

                // https://www.twilio.com/docs/iam/test-credentials#test-sms-messages-parameters-To
                throw;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Unable to send SMS verification code to user");
                throw;
            }
        }
    }
}
