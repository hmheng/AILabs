using System;
namespace AngularImmersiveReader.Models
{
    public class AppSettingsDTO
    {
        public string SubscriptionKey { get; set; }

        public string Endpoint { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Subdomain { get; set; }
    }
}
