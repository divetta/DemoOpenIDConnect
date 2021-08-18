using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace MVCAndJavascriptAuto.Helpers
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
        public string ClientId { get; private set; }

        /// <summary>Gets or sets the assigned client secret</summary>
        public string ClientSecret { get; private set; }

        /// <summary>Gets or sets the URI of authentication server.</summary>
        public string Authority { get; private set; }

        /// <summary>Gets or sets acr_values.</summary>
        public string AcrValues { get; private set; }
        public string AuthorizationEndpoint { get; private set; }
        public string TokenEndpoint { get; private set; }
        public string UserInfoEndpoint { get; private set; }
        public string EndSessionEndpoint { get; private set; }
        public string RevokeTokenEndpoint { get; private set; }
        public string JwksUri { get; private set; }

        /// <summary>Propagates the authentication options from configuration settings(appsettings.json)</summary>
        /// <param name="configuration"></param>
        public async Task PropagateConfigurationAsync(IConfiguration configuration)
        {
            ClientId = configuration["IdentityProvider:ClientId"];
            ClientSecret = configuration["IdentityProvider:ClientSecret"];
            Authority = configuration["IdentityProvider:Authority"];
            AcrValues = configuration["IdentityProvider:AcrValues"];

            var client = new HttpClient();
            //var disco = await client.GetDiscoveryDocumentAsync(Authority);
            var disco = await client.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = Authority,
                    Policy =
                    {
                        ValidateIssuerName = false,
                        ValidateEndpoints = false,
                    },
                }
            );
            if (disco.IsError) throw new Exception(disco.Error);

            AuthorizationEndpoint = disco.AuthorizeEndpoint;
            TokenEndpoint = disco.TokenEndpoint;
            UserInfoEndpoint = disco.UserInfoEndpoint;
            EndSessionEndpoint = disco.EndSessionEndpoint;
            RevokeTokenEndpoint = disco.RevocationEndpoint;
            JwksUri = disco.JwksUri;
        }
    }
}
