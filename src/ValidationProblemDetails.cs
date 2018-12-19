// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using FluentValidation.Results;

    public class ValidationProblemDetails : ProblemDetails
    {
        public ValidationProblemDetails(ValidationResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            this.Errors = new Dictionary<string, List<string>>();
            this.Status = 422;

            foreach (var validationFailure in result.Errors)
            {
                if (this.Errors.ContainsKey(validationFailure.PropertyName))
                {
                    this.Errors[validationFailure.PropertyName].Add(validationFailure.ErrorMessage);
                }

                this.Errors[validationFailure.PropertyName] = new List<string> { validationFailure.ErrorMessage };
            }
        }

        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
