using Microsoft.Extensions.Configuration;
using System;

namespace SimpleApi.Helpers
{
    public sealed class ConfigurationHelper
    {
        private static readonly Lazy<ConfigurationHelper> lazy = new(() => new ConfigurationHelper());

        public static ConfigurationHelper Instance
        {
            get { return lazy.Value; }
        }

        private ConfigurationHelper()
        {
        }

        /// <summary>Gets or sets the assigned client id</summary>
        public string ClientId { get; set; }

        /// <summary>Gets or sets the assigned client secret</summary>
        public string ClientSecret { get; set; }

        /// <summary>Gets or sets the URI of authentication server.</summary>
        public string Authority { get; set; }


        /// <summary>Propagates the authentication options from configuration settings(appsettings.json)</summary>
        /// <param name="configuration"></param>
        public void PropagateConfiguration(IConfiguration configuration)
        {
            ClientId = configuration["IdentityProvider:ClientId"];
            ClientSecret = configuration["IdentityProvider:ClientSecret"];
            Authority = configuration["IdentityProvider:Authority"];
        }
    }
}
