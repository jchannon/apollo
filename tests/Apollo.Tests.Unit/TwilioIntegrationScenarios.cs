// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Net;
    using System.Net.Http;
    using Apollo.Tests.Unit.Sdk;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class TwilioIntegrationScenarios
    {
        private readonly SmsDriver driver;

        public TwilioIntegrationScenarios(ApolloIntegrationFixture services)
        {
            this.driver = new SmsDriver(services);
        }

        [Scenario]
        [Example("+15005550001")]
        [Example("+15005550002")]
        [Example("+15005550003")]
        [Example("+15005550004")]
        [Example("+15005550009")]
        public void CannotSendSMSToInvalidNumber(string phoneNumber, HttpResponseMessage responseMessage)
        {
            "Given I have a user with a verified email and an invalid unverified phone number".x(async () =>
            {
                await this.driver.RegisterUser(emailVerified: true, phoneVerified: false, phoneNumber: phoneNumber);
            });

            "And I can login as the user".x(async () => { await this.driver.Login(); });

            "When I Request an SMS Code".x(async () => { responseMessage = await this.driver.RequestSMSCode(); });

            "Then I get a invalid response code".x(() => { responseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError); });
        }
    }
}
