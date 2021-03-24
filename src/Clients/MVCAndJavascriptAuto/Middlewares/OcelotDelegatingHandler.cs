using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MVCAndJavascriptAuto.Middlewares
{
    public class OcelotDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OcelotDelegatingHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await CheckAccessTokenAsync();

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task CheckAccessTokenAsync()
        {
            var expirestoken = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");
            var expires_at = string.IsNullOrEmpty(expirestoken) ? DateTime.MinValue : DateTime.Parse(expirestoken);

            // Token not valid. Use Refresh Token to get a new access_token and refresh_token.
            if (DateTime.UtcNow > expires_at.ToUniversalTime())
            {
                var refreshClient = new HttpClient();
                var tokenResult = await refreshClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = MyConstants.TokenEndpoint,

                    ClientId = MyConstants.ClientAppClientId,
                    ClientSecret = MyConstants.ClientAppClientSecret,

                    RefreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token")
                });

                if (tokenResult.IsError)
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync("Cookies",
                            new AuthenticationProperties() { RedirectUri = "../index.html" });
                    return;
                }

                var oldIdToken = await _httpContextAccessor.HttpContext.GetTokenAsync("id_token");
                var auth = await _httpContextAccessor.HttpContext.AuthenticateAsync("Cookies");

                var expiresAt = DateTime.UtcNow.AddSeconds(tokenResult.ExpiresIn);

                auth.Properties.StoreTokens(new List<AuthenticationToken>()
                {
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.IdToken,
                        Value = oldIdToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.AccessToken,
                        Value = tokenResult.AccessToken
                    },
                    new AuthenticationToken
                    {
                        Name = OpenIdConnectParameterNames.RefreshToken,
                        Value = tokenResult.RefreshToken
                    },
                    new AuthenticationToken
                    {
                        Name = "expires_at",
                        Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                    }
                });

                await _httpContextAccessor.HttpContext.SignInAsync(auth.Principal, auth.Properties);
            }
        }
    }
}
