namespace Apollo.Settings
{
    public class AppSettings
    {
        public DbSettings Db { get; set; }

        public IdentityServerSettings IdentityServer { get; set; }

        public SmtpSettings Smtp { get; set; }
        
        public TwilioSettings Twilio { get; set; }
    }
}
