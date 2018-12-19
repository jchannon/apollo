// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using Microsoft.AspNetCore.WebUtilities;

    public class ExceptionProblemDetails : ProblemDetails
    {
        public ExceptionProblemDetails(Exception error)
        {
            this.Error = error;
            this.Status = 500;
            this.Type = "https://httpstatuses.com/500";
            this.Title = ReasonPhrases.GetReasonPhrase(500);
        }

        public Exception Error { get; }
    }
}
