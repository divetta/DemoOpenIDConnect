using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MVCAndJavascriptAuto.Helpers
{
    public sealed class CookieAuthenticationOptionsFactory
    {
        private static Lazy<CookieAuthenticationOptionsFactory> lazy = new Lazy<CookieAuthenticationOptionsFactory>(() => new CookieAuthenticationOptionsFactory());

        public static CookieAuthenticationOptionsFactory Instance
        {
            get { return lazy.Value; }
        }

        private CookieAuthenticationOptionsFactory()
        {
        }

        /// <summary>Extends the cookie authentication options with a new event to renew the access token when it is going to be expired and set the cookie lifetime as same as a access token</summary>
        /// <param name="options">options of cookie authentication</param>
        public CookieAuthenticationOptions GetDefaultOptions(CookieAuthenticationOptions options)
        {
            options.Events = new CookieAuthenticationEvents
            {
                // this event is fired everytime the cookie has been validated by the cookie middleware,
                // so basically during every authenticated request
                // the decryption of the cookie has already happened so we have access to the user claims
                // and cookie properties - expiration, etc..
                OnValidatePrincipal = async x =>
                {
                    var identity = (ClaimsIdentity)x.Principal.Identity;
                    //access tokon can only be refreshed if we have refresh token
                    if (identity.FindFirst("refresh_token") != null)
                    {
                        if (!string.IsNullOrEmpty(identity.FindFirst("refresh_token").Value))
                        {
                            // since our cookie lifetime is based on the access token one,
                            // refresh token if we have less than 5 min. from cookie lifetime
                            var expiresUtc = x.Properties.ExpiresUtc;
                            var timeRemaining = DateTime.Parse(identity.FindFirst("access_token_expiresin").Value).ToUniversalTime().Subtract(DateTime.UtcNow);

                            //if (timeRemaining < TimeSpan.FromMinutes(5))
                            if (timeRemaining < TimeSpan.FromMinutes(1))
                            {
                                var accessTokenClaim = identity.FindFirst("access_token");
                                var refreshTokenClaim = identity.FindFirst("refresh_token");
                                var accessTokenExpiresin = identity.FindFirst("access_token_expiresin");
                                string newAccessToken = string.Empty;
                                string newRefreshToken = string.Empty;
                                string newExpiration = string.Empty;
                                try
                                {
                                    // if we have to refresh, grab the refresh token from the claims, and request
                                    // new access token and refresh token
                                    var refreshToken = refreshTokenClaim.Value;

                                    var response = await RequestRefreshTokenAsync(refreshToken);

                                    // Read response body
                                    string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                                    // Converts to dictionary
                                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                                    newAccessToken = tokenEndpointDecoded["access_token"];
                                    newRefreshToken = tokenEndpointDecoded["refresh_token"];
                                    newExpiration = tokenEndpointDecoded["expires_in"];
                                }
                                catch
                                {
                                    //force re-signin
                                    x.RejectPrincipal();

                                    await x.HttpContext.SignOutAsync(
                                        CookieAuthenticationDefaults.AuthenticationScheme);
                                }
                                // everything went right, remove old tokens and add new ones
                                if (!string.IsNullOrEmpty(newAccessToken) && !string.IsNullOrEmpty(newRefreshToken))
                                {
                                    identity.RemoveClaim(accessTokenClaim);
                                    identity.RemoveClaim(refreshTokenClaim);
                                    identity.RemoveClaim(accessTokenExpiresin);

                                    identity.AddClaims(new[]
                                    {
                                        new Claim("access_token", newAccessToken),
                                        new Claim("refresh_token", newRefreshToken),
                                        new Claim("access_token_expiresin", DateTime.Now.AddSeconds(int.Parse(newExpiration)).ToUniversalTime().ToString("o"))
                                    });

                                    x.Properties.Items[".Token.access_token"] = newAccessToken;
                                    // indicate to the cookie middleware to renew the session cookie
                                    // the new lifetime will be the same as the old one, so the alignment
                                    // between cookie and access token is preserved
                                    x.Properties.ExpiresUtc = expiresUtc;
                                    x.ShouldRenew = true;
                                }
                            }
                        }
                        else
                        {
                            //force re-signin
                            x.RejectPrincipal();

                            await x.HttpContext.SignOutAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme);
                        }
                    }

                }
            };

            return options;
        }

        /// <summary>Sends a REST call to the Token API with a refresh token to renew the access token</summary>
        /// <param name="refreshToken">refresh token</param>
        public async Task<HttpResponseMessage> RequestRefreshTokenAsync(string refreshToken)
        {
            HttpClient client = new HttpClient(new HttpClientHandler())
            {
                BaseAddress = new Uri($"{ConfigurationHelper.Instance.TokenEndpoint}")
            };
            var fields = new Dictionary<string, string>
            {
                { "client_id", ConfigurationHelper.Instance.ClientId },
                { "client_secret", ConfigurationHelper.Instance.ClientSecret },
                { "grant_type", "refresh_token"},
                { "refresh_token", refreshToken}
            };
            var response = await client.PostAsync(string.Empty, new FormUrlEncodedContent(fields), default(CancellationToken)).ConfigureAwait(false);
            return response;
        }

    }
}
