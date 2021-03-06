﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace AuthServer
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
                new ApiResource("api1", "My Web API #1")
                {
                    Description = "This is the Web API #1 description.",

                    Scopes = new List<Scope>
                    {
                        new Scope("api1")
                    },

                    ApiSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:5001/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://localhost:4200", // Web address of the client

                    AllowedGrantTypes = GrantTypes.Implicit, // Implicit flow
                    AllowAccessTokensViaBrowser = true, // Self-explanatory, recommended for the implicit flow
                    RequireConsent = true, // The user needs to login to IdentityServer and grant consent
                    AllowOfflineAccess = true, // The client can request refresh tokens
                    AlwaysIncludeUserClaimsInIdToken = true, // Self-explanatory, gives more information about the client
                    AccessTokenType = AccessTokenType.Reference, // Send and receive a reference to a JWT token, do not self contain the JWT token. This makes the HTTP header shorter for a few bytes.

                    RedirectUris =
                    {
                        "http://localhost:4200/index.html",
                        "http://localhost:4200/callback.html",
                        "http://localhost:4200/silent.html",
                        "http://localhost:4200/popup.html",
                        "http://localhost:4200/home"
                    },

                    PostLogoutRedirectUris = { "http://localhost:4200/" },
                    AllowedCorsOrigins = { "http://localhost:4200" },

                    AllowedScopes = { "openid", "profile", "api1" }
                }
            };
        }
    }
}