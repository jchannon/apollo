namespace Apollo.Features.Verification
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Apollo.Settings;
    using IdentityModel.Client;
    using Ironclad.Client;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public class IroncladClaimUpdater
    {
        private readonly HttpClient httpClient;

        private readonly AppSettings appSettings;

        public IroncladClaimUpdater(IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.httpClient = httpClientFactory.CreateClient(Constants.IroncladClient);
            this.appSettings = appSettings;
        }

        public async Task<bool> UpdateIronclad(string userId, string claim, object value)
        {
            var response = await this.httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{this.appSettings.IdentityServer.Authority}/connect/token",

                ClientId = this.appSettings.IroncladClient.ClientId,
                ClientSecret = this.appSettings.IroncladClient.ClientSecret,
                Scope = "auth_api auth_api:write"
            });

            if (response.IsError)
            {
                throw new Exception("Unable to authenticate with Ironclad to update user claims");
            }

            var token = response.AccessToken;

            this.httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);

            var existingUser = await this.httpClient.GetAsync<User>($"/api/users/{userId}");

            if (existingUser == null)
            {
                throw new Exception("Unable to find user to update claims for");
            }

            //TODO Replace with ironclad.client code to update user
            existingUser.Claims.Clear();
            existingUser.Claims[claim] = value;

            var updateResponse = await this.httpClient.PutAsync($"/api/users/{userId}",
                new StringContent(JsonConvert.SerializeObject(existingUser, GetJsonSerializerSettings()), Encoding.UTF8, "application/json"));

            return updateResponse.StatusCode == HttpStatusCode.NoContent;
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
            };

            settings.Converters.Add(new StringEnumConverter());

            return settings;
        }
    }
}
