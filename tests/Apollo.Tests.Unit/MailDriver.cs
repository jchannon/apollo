// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Sdk;
    using Bogus.DataSets;
    using Ironclad.Tests.Sdk;
    using Newtonsoft.Json;
    
    public class MailDriver : IdentityTestDriver
    {
        public MailDriver(ApolloIntegrationFixture services) : base(services) 
        {
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            
            var internet = new Internet();
            this.FromEmailAddress = new MailAddress(internet.Email());
        }
        
        public ApolloIntegrationFixture Services { get; }
        
        public MailAddress FromEmailAddress { get; }

        public async Task SendRequestToVerifyEmailAddress()
        {
            await Task.Yield();
        }
        
        public async Task<string> WaitForEmailWithConfirmationCode()
        {
            var timeBetweenReceiveAttempts = TimeSpan.FromMilliseconds(100);
            using (var client = new HttpClient())
            {
                MailHogMessage message = null; 
                while (message == null)
                {
                    var response = await client.GetAsync(
                        new UriBuilder(this.Services.SmtpServerHttpEndpoint)
                        {
                            Path = "/api/v1/messages"
                        }.Uri);

                    var messages = await MailHogMessage.FromResponse(response.EnsureSuccessStatusCode());
                    
                    //TODO: Improve the predicate to match on more specific attributes. 
                    message = messages.SingleOrDefault(m => m.To.Any(to => to.Equals(new MailAddress(this.CurrentUser.Email))));
                    
                    if (message == null)
                    {
                        await Task.Delay(timeBetweenReceiveAttempts);
                    }
                }

                return message.Body;
            }
        }

        private class MailHogMessage
        {
            public string ID { get; private set; }
            
            public MailAddress From { get; private set; }
            
            public MailAddress[] To { get; private set; }
            
            public string Body { get; private set; }

            public static async Task<MailHogMessage[]> FromResponse(HttpResponseMessage response)
            {
                var messages = new List<MailHogMessage>();
                using(var stream = await response.Content.ReadAsStreamAsync())
                using(var reader = new StreamReader(stream))
                using(var json = new JsonTextReader(reader))
                {
                    while (await json.ReadAsync() && json.TokenType != JsonToken.EndArray)
                    {
                        messages.Add(await FromReader(json));
                    }
                }

                return messages.ToArray();
            }

            private static async Task<MailHogMessage> FromReader(JsonReader json)
            {
                async Task<string> ReadBody()
                {
                    await json.ReadAsync();

                    var body = string.Empty;
                    while (await json.ReadAsync() && json.TokenType != JsonToken.EndObject)
                    {
                        switch (json.Value)
                        {
                            case "Body":
                                body = await json.ReadAsStringAsync();
                                break;

                            default:
                                await json.SkipAsync();
                                break;
                        }
                    }
                    
                    return body;
                }

                async Task<MailAddress> ReadMailAddress()
                {
                    await json.ReadAsync();

                    var mailbox = string.Empty;
                    var domain = string.Empty;
                    while (await json.ReadAsync() && json.TokenType != JsonToken.EndObject)
                    {                        
                        switch (json.Value)
                        {
                            case "Mailbox":
                                mailbox = await json.ReadAsStringAsync();
                                break;

                            case "Domain":
                                domain = await json.ReadAsStringAsync();
                                break;
                            default:
                                await json.SkipAsync();
                                break;
                        }
                    }

                    return !string.IsNullOrEmpty(mailbox) && !string.IsNullOrEmpty(domain)
                        ? new MailAddress(mailbox + "@" + domain)
                        : null;
                }

                var message = new MailHogMessage();
                
                await json.ReadAsync();
                
                while (await json.ReadAsync() && json.TokenType != JsonToken.EndObject)
                {
                    switch (json.Value)
                    {
                        case "ID":
                            message.ID = await json.ReadAsStringAsync();
                            break;
                        case "From":
                            message.From = await ReadMailAddress();
                            break;
                        case "To":
                            var addresses = new List<MailAddress>();
                            while (await json.ReadAsync() && json.TokenType != JsonToken.EndArray)
                            {
                                addresses.Add(await ReadMailAddress());
                            }

                            message.To = addresses.ToArray(); 

                            break;
                        case "Content":
                            message.Body = await ReadBody();
                            break;
                        default:
                            await json.SkipAsync();
                            break;
                    }
                }

                return message;
            }
        }

        public async Task SubmitVerificationCode(string code)
        {
            await Task.Yield();
        }

        public async Task WaitForEmailToBeVerified()
        {
            await Task.Yield();
        }
    }
}