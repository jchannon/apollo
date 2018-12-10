namespace Apollo.Features.Verification.Phone
{
    using System.Threading.Tasks;
    using Apollo.Features.Verification.Phone.PhoneVerification;
    using Apollo.Features.Verification.Phone.PhoneVerificatonSubmission;
    using Carter;
    using Carter.ModelBinding;
    using Microsoft.AspNetCore.Http;

    public class PhoneModule : CarterModule
    {
        public PhoneModule(Handler handler)
        {
            this.RequiresAuthentication();

            this.Post("/phone-verification", async context =>
            {
                var validationResult = context.Request.BindAndValidate<PhoneVerificationMessage>();

                if (!validationResult.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    return;
                }

                var command = new PhoneVerficationCommand(validationResult.Data, context.User, context.RequestAborted);

                var domainResponse = await handler.Handle(command);
                context.Response.StatusCode = domainResponse?.ErrorCode ?? 202;

                await context.Response.WriteAsync("Hello World");
            });

            this.Post("/phone-verification-submission", context =>
            {
                var result = context.Request.BindAndValidate<PhoneVerificationSubmission>();

                if (!result.ValidationResult.IsValid)
                {
                    context.Response.StatusCode = 422;
                    return Task.CompletedTask;
                }

                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
        }
    }
}
