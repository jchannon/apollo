using System;

namespace Apollo.Settings
{
    public class IdentityServerSettings
    {
        public string Authority { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public TimeSpan CacheTimeout { get; set; }
    }
}
