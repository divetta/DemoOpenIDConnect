using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResource("website", new List<string>(){ "website" }),
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                // OpenID Connect implicit flow client (MVC and JS - automatic)
                new Client
                {
                    ClientId = "mvc-and-js-auto",
                    ClientName = "MVC and JS Client - Automatic",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,

                    RedirectUris = { "https://localhost:44306/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44306/signout-callback-oidc" },
                    ClientSecrets = {new Secret("super-secret".ToSha256(),"mvc-secret") },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "website",
                        "api1"
                    }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "divetta",
                    Password = "12345",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name, "Raphael Divetta Bronzo"),
                        new Claim(JwtClaimTypes.GivenName, "Raphael"),
                        new Claim(JwtClaimTypes.FamilyName, "Divetta Bronzo"),
                        new Claim(JwtClaimTypes.Email, "divetta@gmail.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.divetta.com.br"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'Av Paulista', 'locality': 'Sao Paulo', 'postal_code': 0332303, 'country': 'Brazil' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)

                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "simple-api",
                    ApiSecrets = {new Secret("secret".ToSha256(), "simple-api-secret")},
                    Enabled = true,
                    Scopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "website",
                        "api1"
                    }
                }
            };
        }

        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };
    }
}