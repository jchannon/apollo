// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Newtonsoft.Json.Linq;

    public static class HttpResponseExtensions
    {
        public static async Task ShouldBeRejectedWithMatchingTypeField(this HttpResponseMessage response, string type)
        {
            var body = await response.Content.ReadAsStringAsync();

            var jsonResponse = JObject.Parse(body);

            var errorType = jsonResponse["type"].ToString();

            errorType.Should().Be(type);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
