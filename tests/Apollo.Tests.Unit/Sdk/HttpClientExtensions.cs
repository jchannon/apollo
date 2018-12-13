// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string uri, T body)
        {
            var request = JsonConvert.SerializeObject(body);
            return client.PostAsync(uri, new StringContent(request, Encoding.UTF8, "application/json"));
        }
    }
}
