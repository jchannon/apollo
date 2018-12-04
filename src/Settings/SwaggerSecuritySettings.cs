using System.Collections.Generic;

namespace Apollo.Settings
{
    public class SwaggerSecuritySettings
    {
        public string OAuth2ClientId { get; set; }
        public Dictionary<string, string> OAuth2Scopes { get; set; }
        public string AuthorizeEndpoint { get; set; }
    }
}
