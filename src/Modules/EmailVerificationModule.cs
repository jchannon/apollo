using Apollo.Models;
using Carter;
using Carter.ModelBinding;
using Carter.Response;

namespace Apollo.Modules
{
    public class EmailVerificationModule : CarterModule
    {
        public EmailVerificationModule() : base("/emailVerification")
        {
            this.RequiresAuthentication();

            Post("/", async context =>
            {
                // todo
            });

            Post("/confirmation", async (req, res, routeData) =>
            {
                var result = req.BindAndValidate<EmailConfirmationModel>();

                if (!result.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(result.ValidationResult.GetFormattedErrors());
                    return;
                }

                // todo
            });
        }
    }
}
