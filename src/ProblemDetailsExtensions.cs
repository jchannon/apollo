// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public static class ProblemDetailsExtensions
    {
        public static IServiceCollection AddProblemDetails(this IServiceCollection services, Action<ProblemDetailsOptions> options)
        {
            var problemDetailsOptions = new ProblemDetailsOptions();
            options(problemDetailsOptions);

            services.AddSingleton(problemDetailsOptions);
            return services;
        }
    }
}
