// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Apollo.Tests.Unit.Sdk;
    using Bogus.DataSets;
    using Xunit;

    [Collection(nameof(ApolloIntegrationCollection))]
    public class MailDriverTests
    {
        public MailDriverTests(ApolloIntegrationFixture services)
        {
            this.driver = new MailDriver(services);
        }

        private readonly MailDriver driver;

        private void SendMailMessage(MailMessage message)
        {
            using (var client = new SmtpClient(
                this.driver.Services.SmtpServerEndpoint.Host,
                this.driver.Services.SmtpServerEndpoint.Port))
            {
                client.Send(message);
            }
        }

        [Fact]
        public async Task WaitForEmailWithConfirmationCodeHasExpectedBehavior()
        {
            // TODO: Once templating is introduced this test should reflect that.
            // We could use our email sender implementation.

            var lorem = new Lorem();
            var code = lorem.Letter(5);

            using (var message = new MailMessage(this.driver.FromEmailAddress, new MailAddress(this.driver.CurrentUser.Email))
            {
                Body = code
            })
            {
                this.SendMailMessage(message);
            }

            var result = await this.driver.WaitForEmailWithConfirmationCode();

            Assert.Equal(code, result.Trim());
        }
    }
}
