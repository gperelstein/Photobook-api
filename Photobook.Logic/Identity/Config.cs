using IdentityModel;
using IdentityServer4.Models;
using Photobook.Common.Configuration;
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

        public static IEnumerable<ApiResource> GetApis(IdentityOptions identityOptions)
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = identityOptions.ClientName,
                    ApiSecrets =
                    {
                        new Secret(identityOptions.ClientSecret.Sha256())
                    },
                    UserClaims = { JwtClaimTypes.Name, JwtClaimTypes.Email },
                    Scopes = {"openid", "profile", "read", "write", "delete", "regular"}
                }
            };
        }

        public static IEnumerable<Client> GetClients(IdentityOptions identityOptions)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = identityOptions.ClientId,
                    ClientSecrets = { new Secret(identityOptions.ClientSecret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "openid", "profile", "read", "write", "delete", "regular" },
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
                new ApiScope(name: "regular",   displayName: "Regular data access"),
                new ApiScope(name: "read",   displayName: "Read your data."),
                new ApiScope(name: "write",  displayName: "Write your data."),
                new ApiScope(name: "delete", displayName: "Delete your data.")
            };
        }
    }
}
