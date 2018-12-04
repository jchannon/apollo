namespace Apollo
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public class PhoneVerificationCommandHandler : CommandHandler<PhoneVerficationCommand>
    {
        private readonly AppSettings appSettings;

        private readonly HttpClient client = new HttpClient();

        private bool emailVerified;

        private static readonly Dictionary<string, string> recordedPhoneNumbers = new Dictionary<string, string>();

        public PhoneVerificationCommandHandler(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        protected override async Task<Error> Handle(PhoneVerficationCommand command)
        {
            await this.GetUserClaims(command);
            if (!this.emailVerified)
            {
                return new Error { ErrorCode = 400 };
            }

            if (recordedPhoneNumbers.ContainsKey(command.VerificationMessage.Phonenumber))
            {
                return new Error { ErrorCode = 400 };
            }

            recordedPhoneNumbers.Add(command.VerificationMessage.Phonenumber, string.Empty);

            return null;
        }

        private async Task GetUserClaims(PhoneVerficationCommand command)
        {
            this.client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(command.AuthorizationToken);
            using (var response = await this.client.GetAsync($"{this.appSettings.IdentityServer.Authority}/connect/userinfo", command.CancellationToken).ConfigureAwait(false))
            {
                var jsonClaims = await response.Content.ReadAsStringAsync();
                var claimsObject = JObject.Parse(jsonClaims);
                this.emailVerified = bool.Parse(claimsObject["email_verified"].ToString());
            }
        }
    }
}
