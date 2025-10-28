using IdentityServer8;
using IdentityServer8.Models;
using System.Collections.Generic;

namespace AngularSPAWebAPI
{
    public class Config
    {
        // Identity resources (used by UserInfo endpoint).
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", new List<string> { "role" })
            };
        }

        // Api scopes.
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("WebAPI", "WebAPI scope") {
                    UserClaims = { "role", "sub", "name" }
                }
            };
        }

        // Api resources.
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("WebAPI", "WebAPI resource") {
                    Scopes = { "WebAPI" },
                    UserClaims = { "role", "sub", "name" }
                }
            };
        }

        // Clients want to access resources.
        public static IEnumerable<Client> GetClients()
        {
            // Clients credentials.
            return new List<Client>
            {
                // http://docs.identityserver.io/en/release/reference/client.html.
                new Client
                {
                    ClientId = "AngularCBAS",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Resource Owner Password Credential grant.
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.
                    IdentityTokenLifetime = 3600, //Lifetime to identity token in seconds (defaults to 300 seconds / 5 minutes)
                    AccessTokenLifetime = 7200, // Lifetime of access token in seconds.
                    
                    // Use JWT access tokens and include user claims
                    AccessTokenType = AccessTokenType.Jwt,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    IncludeJwtId = true,

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "WebAPI"
                    },
                    AllowOfflineAccess = true, // For refresh token.
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    AbsoluteRefreshTokenLifetime = 2592000,
                    SlidingRefreshTokenLifetime = 1296000,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200"
                    } // Only for development.
                }
            };
        }
    }
}
