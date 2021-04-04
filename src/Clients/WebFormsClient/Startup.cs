using IdentityModel;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(WebFormsClient.Startup))]

namespace WebFormsClient
{
    public class Startup
    {
        string clientId = "web-forms";
        string clientSecret = "super-secret-webforms";
        string redirectUri = "https://localhost:44304/";
        string authority = "https://localhost:44301";

        /// <summary>
        /// Configure OWIN to use OpenIdConnect 
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "cookie"
            }); 
            
            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Authority = authority,
                RedirectUri = redirectUri,
                ResponseType = OpenIdConnectResponseType.Code,
                ResponseMode = "query",
                Scope = "openid profile email address phone authorization_group entitlement_group",

                SignInAsAuthenticationType = "cookie",

                UseTokenLifetime = false,

                SaveTokens = true,
                RedeemCode = true,
               

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = OnAuthenticationFailed,
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                        {
                            // generate code verifier and code challenge
                            var codeVerifier = CryptoRandom.CreateUniqueId(32);

                            string codeChallenge;
                            using (var sha256 = SHA256.Create())
                            {
                                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                                codeChallenge = Base64Url.Encode(challengeBytes);
                            }

                            // set code_challenge parameter on authorization request
                            n.ProtocolMessage.SetParameter("code_challenge", codeChallenge);
                            n.ProtocolMessage.SetParameter("code_challenge_method", "S256");

                            // remember code verifier in cookie (adapted from OWIN nonce cookie)
                            // see: https://github.com/scottbrady91/Blog-Example-Classes/blob/master/AspNetFrameworkPkce/ScottBrady91.BlogExampleCode.AspNetPkce/Startup.cs#L85
                            RememberCodeVerifier(n, codeVerifier);
                        }

                        return Task.CompletedTask;
                    },
                    AuthorizationCodeReceived = n =>
                    {
                        // get code verifier from cookie
                        // see: https://github.com/scottbrady91/Blog-Example-Classes/blob/master/AspNetFrameworkPkce/ScottBrady91.BlogExampleCode.AspNetPkce/Startup.cs#L102
                        var codeVerifier = RetrieveCodeVerifier(n);

                        // attach code_verifier on token request
                        n.TokenEndpointRequest.SetParameter("code_verifier", codeVerifier);

                        return Task.CompletedTask;
                    }
                }
            }
            );
        }

        private void RememberCodeVerifier(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n, string codeVerifier)
        {
            var properties = new AuthenticationProperties();
            properties.Dictionary.Add("cv", codeVerifier);
            n.Options.CookieManager.AppendResponseCookie(
                n.OwinContext,
                GetCodeVerifierKey(n.ProtocolMessage.State),
                Convert.ToBase64String(Encoding.UTF8.GetBytes(n.Options.StateDataFormat.Protect(properties))),
                new CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    HttpOnly = true,
                    Secure = n.Request.IsSecure,
                    Expires = DateTime.UtcNow + n.Options.ProtocolValidator.NonceLifetime
                });
        }

        private string RetrieveCodeVerifier(AuthorizationCodeReceivedNotification n)
        {
            string key = GetCodeVerifierKey(n.ProtocolMessage.State);

            string codeVerifierCookie = n.Options.CookieManager.GetRequestCookie(n.OwinContext, key);
            if (codeVerifierCookie != null)
            {
                var cookieOptions = new CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    HttpOnly = true,
                    Secure = n.Request.IsSecure
                };

                n.Options.CookieManager.DeleteCookie(n.OwinContext, key, cookieOptions);
            }

            var cookieProperties = n.Options.StateDataFormat.Unprotect(Encoding.UTF8.GetString(Convert.FromBase64String(codeVerifierCookie)));
            cookieProperties.Dictionary.TryGetValue("cv", out var codeVerifier);

            return codeVerifier;
        }

        private string GetCodeVerifierKey(string state)
        {
            using (var hash = SHA256.Create())
            {
                return OpenIdConnectAuthenticationDefaults.CookiePrefix + "cv." + Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(state)));
            }
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}
