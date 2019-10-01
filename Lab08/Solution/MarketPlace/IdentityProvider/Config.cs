// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityProvider
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("backend", "MarketPlace REST API"){ UserClaims = { JwtClaimTypes.Name } }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "frontend",
                    ClientName = "MarketPlace JavaScript Client",
                    ClientUri = "http://localhost:8080",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:8080",
                        "http://localhost:8080/callback",
                        "http://localhost:8080/silent",
                        "http://localhost:8080/popup",
                    },

                    PostLogoutRedirectUris = { "http://localhost:8080" },
                    AllowedCorsOrigins = { "http://localhost:8080" },

                    AllowedScopes = { "openid", "profile", "backend" }
                }
            };
        }
    }
}