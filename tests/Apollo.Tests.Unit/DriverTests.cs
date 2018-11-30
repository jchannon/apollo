// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Sdk;
    using Bogus.DataSets;
    using Ironclad.Tests.Sdk;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class DriverTests
    {
        private readonly Driver driver;

        public DriverTests(ApolloIntegrationFixture services, AuthenticationFixture authentication)
        {
            this.driver = new Driver(services, authentication);
        }

        [Fact]
        public async Task WaitForEmailWithConfirmationCodeHasExpectedBehavior()
        {
            // TODO: Once templating is introduced this test should reflect that.
            // We could use our email sender implementation.
            
            var lorem = new Lorem();
            var code = lorem.Letter(5);
            
            using (var message = new MailMessage(this.driver.FromEmailAddress, this.driver.ToEmailAddress)
            {
                Body = code
            })
            {
                this.SendMailMessage(message);
            }

            var result = await this.driver.WaitForEmailWithConfirmationCode();
            
            Assert.Equal(code, result.Trim());
        }

        private void SendMailMessage(MailMessage message)
        {
            using (var client = new SmtpClient(
                this.driver.Services.SmtpServerEndpoint.Host,
                this.driver.Services.SmtpServerEndpoint.Port))
            {
                client.Send(message);
            }
        }
    }
}