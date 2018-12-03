﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Apollo.Tests.Unit.Sdk;
using Bogus.DataSets;
using FluentAssertions;
using IdentityModel.Client;
using Ironclad.Tests.Sdk;
using Newtonsoft.Json;

namespace Apollo.Tests.Unit
{
    public abstract class IdentityTestDriver
    {
        private readonly Internet internet;
        private readonly PhoneNumbers phoneNumbers;
        private readonly HttpClient ironcladClient;
        
        protected ApolloIntegrationFixture Services { get; }
        protected HttpClient ApolloClient { get; }
        
        protected IdentityTestDriver(ApolloIntegrationFixture services)
        {
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            
            this.ironcladClient = new HttpClient(services.IdentityAuthorityAdminHandler)
            {
                BaseAddress = services.IdentityAuthority
            };
            
            this.ApolloClient = new HttpClient
            {
                BaseAddress = services.ApolloEndpoint
            };

            this.internet = new Internet();
            this.phoneNumbers = new PhoneNumbers();
        }

        public async Task RegisterUser()
        {
            var email = internet.Email();
            
            var currentUser = new User
            {
                Username = email,
                Password = internet.Password(),
                Email = email,
                PhoneNumber = this.phoneNumbers.PhoneNumber(),
            };

            var response = await this.ironcladClient.PostAsync("/api/users",
                new StringContent(JsonConvert.SerializeObject(currentUser), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }
        
        public async Task Login()
        {
            var url = new RequestUrl(this.Services.IdentityAuthority + "connect/authorize")
                .CreateAuthorizeUrl(ApolloIntegrationFixture.ApolloAuthClientId, "id_token token", $"openid profile {ApolloIntegrationFixture.ApolloAuthApiIdentifier}",
                    $"{this.Services.ApolloEndpoint}/redirect", "state", "nonce");

            var automation = new BrowserAutomation("admin", "password");
            await automation.NavigateToLoginAsync(url).ConfigureAwait(false);
            var authorizeResponse = await automation.LoginToAuthorizationServerAndCaptureRedirectAsync().ConfigureAwait(false);

            // assert
            authorizeResponse.IsError.Should().BeFalse();
            this.ApolloClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizeResponse.AccessToken);
        }
    }
}