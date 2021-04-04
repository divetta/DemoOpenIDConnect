using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MVCAndJavascriptAuto.Helpers
{
    public sealed class OidcOptionsFactory
    {
        private static Lazy<OidcOptionsFactory> lazy = new Lazy<OidcOptionsFactory>(() => new OidcOptionsFactory());

        public static OidcOptionsFactory Instance
        {
            get { return lazy.Value; }
        }

        private OidcOptionsFactory()
        {
        }

        /// <summary>Sets default openId connection options, and handles the logout activity</summary>
        /// <param name="options">object of OpenIdConnectOptions</param>
        public OpenIdConnectOptions GetDefaultOptions(OpenIdConnectOptions options)
        {
            OidcConfigurationFactory.Instance.SetDefaultOidcConfiguration(options);

            // we use the authorisation code flow, so only asking for a code
            options.ResponseType = OpenIdConnectResponseType.Code;

            // adding offline_access to get a refresh token
            // Not supported currently on PingFederate authentication server at Daimler
            //options.Scope.Add("offline_access");

            // we want IdSrv to post the data back to us
            options.ResponseMode = "form_post";


            // when the identity has been created from the data we receive,
            // persist it with this authentication scheme, hence in a cookie
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            // Saves tokens to the AuthenticationProperties
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            // using this property would align the expiration of the cookie
            // with the expiration of the identity token
            options.UseTokenLifetime = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = true,
                IssuerSigningKeys = options.Configuration.JsonWebKeySet.Keys
            };
            options.Events = new OpenIdConnectEvents
            {
                // handle IDP redirection
                OnRedirectToIdentityProvider = (context) =>
                {
                    if (!string.IsNullOrEmpty(ConfigurationHelper.Instance.AcrValues))
                    {
                        context.ProtocolMessage.Parameters.Add("acr_values", ConfigurationHelper.Instance.AcrValues);
                    }
                    return Task.CompletedTask;
                },



                // that event is called after the OIDC middleware received the auhorisation code,
                // redeemed it for an access token and a refresh token,
                // and validated the identity token
                OnTokenValidated = x =>
                {
                    if (!string.IsNullOrEmpty(ConfigurationHelper.Instance.AcrValues))
                    {
                        var jwtIdToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(x.TokenEndpointResponse.IdToken);
                        string claimValue = jwtIdToken.Claims.Where(c => c.Type == "acr").FirstOrDefault()?.Value;
                        if (claimValue == null || !claimValue.Equals(ConfigurationHelper.Instance.AcrValues))
                        {
                            throw new Exception("ACR value invalid");
                        }
                    }
                    // store both access and refresh token in the claims - hence in the cookie
                    var identity = (ClaimsIdentity)x.Principal.Identity;
                    identity.AddClaims(new[]
                    {
                            new Claim("access_token", x.TokenEndpointResponse.AccessToken),
                            new Claim("refresh_token", x.TokenEndpointResponse.RefreshToken)
                        });

                    // so that we don't issue a session cookie but one with a fixed expiration
                    x.Properties.IsPersistent = true;

                    // align expiration of the cookie with expiration of the
                    // access token
                    x.Properties.ExpiresUtc = DateTime.Now.AddSeconds(int.Parse(x.TokenEndpointResponse.ExpiresIn)).ToUniversalTime();
                    return Task.CompletedTask;
                },


                // handle the logout redirection 
                OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    // var logoutUri = ConfigurationHelper.Instance.LogoutRedirectUrl;
                    string accessToken = context.HttpContext.User.Claims.Where(c => c.Type == "access_token").FirstOrDefault()?.Value;
                    string refreshToken = context.HttpContext.User.Claims.Where(c => c.Type == "refresh_token").FirstOrDefault()?.Value;

                    HttpWebRequest endSessionRequest = (HttpWebRequest)WebRequest.Create(context.Options.Configuration.EndSessionEndpoint);
                    endSessionRequest.Method = "GET";
                    endSessionRequest.Headers.Add(string.Format("Authorization: Bearer {0}", accessToken));
                    endSessionRequest.ContentType = "application/x-www-form-urlencoded";
                    endSessionRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*;q=0.8";

                    // Get the response
                    string responseText = string.Empty;
                    WebResponse endSessionResponse = null;
                    try
                    {
                        endSessionResponse = endSessionRequest.GetResponse();

                        using (StreamReader rReader = new StreamReader(endSessionResponse.GetResponseStream()))
                        {
                            responseText = rReader.ReadToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        responseText = ex.Message;
                    }
                    finally
                    {
                        if (endSessionResponse != null)
                        {
                            endSessionResponse.Close();
                        }
                    }
                    //Revoke refresh token if exists
                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        string refreshRevokerequestBody = $"token_type_hint=refresh_token&token={refreshToken}";
                        AuthenticationHelper.CallEndPoint(ConfigurationHelper.Instance.RevokeTokenEndpoint, refreshRevokerequestBody, AuthenticationHelper.EncodeCredential(ConfigurationHelper.Instance.ClientId, ConfigurationHelper.Instance.ClientSecret));
                    }
                    //Revoke access token
                    string accessRevokerequestBody = $"token_type_hint=access_token&token={accessToken}";
                    AuthenticationHelper.CallEndPoint(ConfigurationHelper.Instance.RevokeTokenEndpoint, accessRevokerequestBody, AuthenticationHelper.EncodeCredential(ConfigurationHelper.Instance.ClientId, ConfigurationHelper.Instance.ClientSecret));


                    //context.Response.Redirect(logoutUri);
                    //context.HttpContext.Response.Body.Write(StringToByteArray(responseText), 0, StringToByteArray(responseText).Length);
                    //context.HandleResponse();


                    return Task.CompletedTask;
                }
            };

            return options;
        }
        private byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }
    }
}
