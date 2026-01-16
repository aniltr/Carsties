using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new("auctionApp", "Auction app full access"),
        ];

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new() {
                ClientId = "postman",
                ClientName = "Postman",
                ClientSecrets =  { new Secret ("NotASecret".Sha256()) },
                AllowedGrantTypes = {GrantType.ResourceOwnerPassword},
                RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
                AllowedScopes = {"openid", "profile", "auctionApp"}
            },
            new()
            {
                ClientId = "nextApp",
                ClientName = "nextApp",
                ClientSecrets = { new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RedirectUris = {"http://localhost:3000/api/auth/callback/id-server"},
                AllowedScopes = {"openid", "profile", "auctionApp"},
                RequirePkce = false,
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600*24*30
            }
        };
}
