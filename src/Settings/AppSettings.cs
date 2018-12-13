// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

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
