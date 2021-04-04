using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public class AuthorizationGroup : IdentityResource
    {
        public AuthorizationGroup()
        {
            Name = "authorization_group";
            DisplayName = "Authorization group";
            Emphasize = true;
            UserClaims = new List<string>() { "authorization_group" };
        }
    }

    public class EntitlementGroup : IdentityResource
    {
        public EntitlementGroup()
        {
            Name = "entitlement_group";
            DisplayName = "Entitlement group";
            Emphasize = true;
            UserClaims = new List<string>() { "entitlement_group" };
        }
    }

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
                new IdentityResources.Phone(),
                new AuthorizationGroup(),
                new EntitlementGroup(),
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
                    ClientId = "cd2c4616-d527-46b8-b5db-9511da1c8545",
                    ClientName = "MVC and JS Client - Automatic",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,

                    RedirectUris = { "https://localhost:44306/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44306/signout-callback-oidc" },
                    ClientSecrets = {new Secret("024b6081-7d02-4f64-bf22-cb740e8c2208".ToSha256()) },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        "authorization_group",
                        "entitlement_group"
                    }
                },
                new Client
                {
                    ClientId = "web-forms",
                    ClientName = "Web Forms",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,

                    RedirectUris = { "https://localhost:44304/" },
                    PostLogoutRedirectUris = { "https://localhost:44304/" },
                    ClientSecrets = {new Secret("super-secret-webforms".ToSha256()) },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        "authorization_group",
                        "entitlement_group"
                    }
                },
                new Client
                {
                    ClientId = "123456",
                    ClientName = "Web Forms - Daimler",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AccessTokenType = AccessTokenType.Reference,

                    RedirectUris = { "https://localhost:44320/signin-oidc/" },
                    PostLogoutRedirectUris = { "https://localhost:44320" },
                    ClientSecrets = {new Secret("654321".ToSha256()) },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        "authorization_group",
                        "entitlement_group"
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
                    SubjectId = "divetta",
                    Username = "divetta",
                    Password = "12345",

                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Name, "RAPHAEL BRONZO"),
                        new Claim(JwtClaimTypes.GivenName, "Raphael"),
                        new Claim(JwtClaimTypes.FamilyName, "Bronzo"),
                        new Claim(JwtClaimTypes.Email, "divetta@gmail.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.PhoneNumber, "+5511999999999"),
                        new Claim(JwtClaimTypes.PhoneNumberVerified, "false", ClaimValueTypes.Boolean),
                        //new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'Av Paulista', 'locality': 'Sao Paulo', 'postal_code': 0332303, 'country': 'Brazil' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                        new Claim("authorization_group", "BMBVU.AcessoGeral"),
                        new Claim("entitlement_group", "BMBVU.Bens-Imoveis-Escrita"),
                        new Claim("entitlement_group", "BMBVU.Bens-Imoveis-Leitura"),
                        new Claim("entitlement_group", "BMBVU.Bens-Veiculos-Escrita"),
                        new Claim("entitlement_group", "BMBVU.Bens-Veiculos-Leitura"),
                        new Claim("entitlement_group", "BMBVU.ControleDeAcesso-Usuarios-Escrita"                              ),
                        new Claim("entitlement_group", "BMBVU.ControleDeAcesso-Usuarios-Leitura"                              ),
                        new Claim("entitlement_group", "BMBVU.Institucional-Banners-Escrita"                                  ),
                        new Claim("entitlement_group", "BMBVU.Institucional-Banners-Leitura"                                  ),
                        new Claim("entitlement_group", "BMBVU.Institucional-Contatos-Escrita"                                 ),
                        new Claim("entitlement_group", "BMBVU.Institucional-Contatos-Leitura"                                 ),
                        new Claim("entitlement_group", "BMBVU.Institucional-FAQ-Escrita"                                      ),
                        new Claim("entitlement_group", "BMBVU.Institucional-FAQ-Leitura"                                      ),
                        new Claim("entitlement_group", "BMBVU.Relatorios-Bens-Leitura"                                        ),
                        new Claim("entitlement_group", "BMBVU.Relatorios-Desativados-Leitura"                                 ),
                        new Claim("entitlement_group", "BMBVU.Relatorios-IntencaoDeCompra-Leitura"                            ),
                        new Claim("entitlement_group", "BMBVU.Relatorios-PropostaDeMaiorValor-Escrita"                        ),
                        new Claim("entitlement_group", "BMBVU.Relatorios-PropostaDeMaiorValor-Leitura"                        ),
                        new Claim("entitlement_group", "BMBVU.Relatorios-PropostaDeMaiorValorArquivos-Leitura"                ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Auditoria-Leitura"             ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Deposito-Escrita"              ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Deposito-Leitura"              ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Despesas-Escrita"              ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Despesas-Leitura"              ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Escrita"                       ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Laudo-Escrita"                 ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Laudo-Leitura"                 ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Leitura"                       ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Liberacao-Escrita"             ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Liberacao-Leitura"             ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Proposta-Escrita"              ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Proposta-Leitura"              ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Venda-Escrita"                 ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-CadastroVeiculos-Venda-Leitura"                 ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Parametrizacoes-AlcadaPropostaCompra-Escrita"   ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Parametrizacoes-AlcadaPropostaCompra-Leitura"   ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Parametrizacoes-AlcadaPropostaLiberacao-Escrita"),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Parametrizacoes-AlcadaPropostaLiberacao-Leitura"),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-PesquisaBensFrota-Leitura"             ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Relatorios-BensDisponiveis-Leitura"    ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Relatorios-BensXAnuncios-Leitura"      ),
                        new Claim("entitlement_group", "BMBVU.VeiculosDaFrota-Relatorios-PagamentosPendentes-Leitura")

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
                    Name = "e2f59bbd-ef43-4a04-82f8-1b358f74675e",
                    ApiSecrets = {new Secret("def1743d-dbc3-45ce-b3fa-86b3b42c73d3".ToSha256())},
                    Enabled = true,
                    Scopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        "authorization_group",
                        "entitlement_group"
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