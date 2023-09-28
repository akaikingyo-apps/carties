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
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction app full access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "postman",
                ClientName = "Postman",

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // use password directly!
                ClientSecrets = { new Secret("NotASecret".Sha256()) }, 
                AllowedScopes = { "openid", "profile", "auctionApp" },
                RedirectUris = { "https://www.getpostman.com/oauth2/callback" },
            },
            new Client
            {
                ClientId = "nextApp",
                ClientName = "nextApp",

                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials, // no secret sent to browser
                ClientSecrets = { new Secret("secret".Sha256()) },  
                RequirePkce = false, // if phone app which we cannot store the secret key
                AllowedScopes = { "openid", "profile", "auctionApp" },
                AllowOfflineAccess = true, // allow refresh token functions
                AccessTokenLifetime = 3600*24*30, // last 1 month for development
                AlwaysIncludeUserClaimsInIdToken = true,
                RedirectUris = { "http://localhost:3000/api/auth/callback/id-server" },
            },
        };
}
