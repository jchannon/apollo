using System;
using System.Threading.Tasks;

namespace Apollo.Features.Verification
{
    public enum VerificationType
    {
        Email,
        Phone
    }
   
    public class VerificationCodeManager
    {
        public async Task GenerateCode(VerificationType type, string userId, Func<VerificationCode, Task> dispatch)
        {
            //generate the code
            //store it in table storage
            //dispatch it using dispatch(code)
            var code = VerificationCode.Generate();
            await dispatch(code);
        }

        public Task VerifyCode(VerificationType type, string userId, VerificationCode code)
        {
            //look up the code in storage
            //do validation checks
            //done
            return Task.CompletedTask;
        }
    }
}
