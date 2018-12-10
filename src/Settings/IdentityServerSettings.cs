namespace Apollo.Settings
{
    using System;

    public class IdentityServerSettings
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public string ApiSecret { get; set; }

        public TimeSpan CacheTimeout { get; set; }
    }
}
