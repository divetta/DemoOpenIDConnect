using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace MVCAndJavascriptAuto.Helpers
{
    public sealed class OidcConfigurationFactory
    {
        private static Lazy<OidcConfigurationFactory> lazy = new Lazy<OidcConfigurationFactory>(() => new OidcConfigurationFactory());

        public static OidcConfigurationFactory Instance
        {
            get { return lazy.Value; }
        }

        private OidcConfigurationFactory()
        {
        }

        /// <summary>Sets default openID connection options</summary>
        /// <param name="options"></param>
        public void SetDefaultOidcConfiguration(OpenIdConnectOptions options)
        {
            options.Authority = ConfigurationHelper.Instance.Authority;
            options.ClientId = ConfigurationHelper.Instance.ClientId;
            options.ClientSecret = ConfigurationHelper.Instance.ClientSecret;

            OpenIdConnectConfiguration conf = new OpenIdConnectConfiguration();

            conf.AuthorizationEndpoint = ConfigurationHelper.Instance.AuthorizationEndpoint;
            conf.TokenEndpoint = ConfigurationHelper.Instance.TokenEndpoint;
            conf.UserInfoEndpoint = ConfigurationHelper.Instance.UserInfoEndpoint;
            conf.EndSessionEndpoint = ConfigurationHelper.Instance.EndSessionEndpoint;

            var request = WebRequest.CreateHttp(ConfigurationHelper.Instance.JwksUri);
            StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();
            conf.JsonWebKeySet = new JsonWebKeySet(responseData);

            options.Configuration = conf;
        }
    }
}
