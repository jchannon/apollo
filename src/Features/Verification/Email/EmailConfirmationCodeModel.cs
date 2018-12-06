namespace Apollo.Features.Verification.Email
{
    /// <summary>
    /// Email confirmation request details
    /// </summary>
    public class EmailConfirmationCodeModel
    {
        /// <summary>
        /// Email confirmation code
        /// </summary>
        public string Code { get; set; }
    }
}
