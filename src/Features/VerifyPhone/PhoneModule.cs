namespace Apollo.Features.VerifyPhone
{
    using System.Threading.Tasks;
    using Apollo.Features.VerifyPhone.PhoneVerification;
    using Apollo.Features.VerifyPhone.PhoneVerificatonSubmission;
    using Carter;
    using Carter.ModelBinding;
    using Microsoft.AspNetCore.Http;

    public class PhoneModule : CarterModule
    {
        public PhoneModule(Handler handler)
        {
            this.RequiresAuthentication();

            this.Get("/status", context => Task.CompletedTask);

            this.Post("/phone-verification", async context =>
            {
                var validationResult = context.Request.BindAndValidate<VerificationMessage>();

                if (!validationResult.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    return;
                }

                var command = new PhoneVerficationCommand(validationResult.Data, context.Request.Headers["Authorization"], context.RequestAborted);

                var domainResponse = await handler.Handle(command);
                context.Response.StatusCode = domainResponse?.ErrorCode ?? 202;

                await context.Response.WriteAsync("Hello World");
            });

            this.Post("/phone-verification-submission", async context =>
            {
                var result = context.Request.BindAndValidate<VerificationSubmission>();

                if (!result.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    return;
                }

                context.Response.StatusCode = 204;
            });
        }
    }
}
