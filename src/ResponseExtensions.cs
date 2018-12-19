// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System.Threading.Tasks;
    using Carter.Response;
    using Microsoft.AspNetCore.Http;

    public static class ResponseExtensions
    {
        public static async Task WriteProblemDetails(this HttpResponse response, ProblemDetails problemDetails)
        {
            response.StatusCode = problemDetails.Status ?? 500;
            response.ContentType = "application/problem+json";
            await response.AsJson(problemDetails);
        }
    }
}
