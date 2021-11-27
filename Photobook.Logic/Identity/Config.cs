using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Photobook.Logic.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResources.Address()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "photobookweb",
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email },
                    Scopes = {"openid", "profile", "read", "write", "delete"}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "service.client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "openid", "profile", "read", "write", "delete" },
                    AllowOfflineAccess = true,
                    IdentityTokenLifetime = 60 * 60 * 24,
                    AccessTokenLifetime = 60 * 60 * 24,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    EnableLocalLogin = true,
                    Enabled = true,
                    RequireClientSecret = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                }
            };
        }

        public static IEnumerable<ApiScope> GetScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data.")
            };
        }
    }
}
