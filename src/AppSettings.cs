namespace Apollo
{
    using System;

    public class AppSettings
    {
        public IdentityServerSettings IdentityServer { get; set; }
    }
    
    public class IdentityServerSettings
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }

        public string ApiSecret { get; set; }

        public TimeSpan CacheTimeout { get; set; }
    }

}