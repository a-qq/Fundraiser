// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace Fundraiser.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(
                    "roles", //scopename
                    "Your role(s) and school", //display name
                    new List<string>() { "role" } ), //list of claims
                new IdentityResource(
                    "schools", //scopename
                    "Your role(s) and school", //display name
                    new List<string>() { "school_id" }), //list of claims
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("fundraiserapi.fullaccess", "Fundraiser API"),
                //new ApiScope(IdentityServerConstants.LocalApi.ScopeName + ".read", IdentityServerConstants.LocalApi.ScopeName + ".read"),
                //new ApiScope(IdentityServerConstants.LocalApi.ScopeName + ".manage", IdentityServerConstants.LocalApi.ScopeName + ".manage")
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                
                // allows full access (via scope) to the api1 resource (which will end up as audience value)
                new ApiResource("fundraiserapi", "Fundraiser API", new List<string>() { "role", "school_id" }) 
                {
                    Scopes = { "fundraiserapi.fullaccess" }, // "roles"
                    //ApiSecrets = { new Secret("apisecret".Sha256()) }
                },

                //new ApiResource(IdentityServerConstants.LocalApi.ScopeName, new List<string>() { "role", "organization" })
                //{
                //    Scopes = {
                //        IdentityServerConstants.LocalApi.ScopeName + ".read",
                //        IdentityServerConstants.LocalApi.ScopeName + ".manage" }
                //}
                
                // // allows full access (via scope) to the api1 resource (which will end up as audience value)
                //new ApiResource("api1", "API1", new[] { "profile", "email" })
                //{
                //    Scopes = { "api1.fullaccess" },
                //    ApiSecrets = { new Secret("apisecret".Sha256()) }
                //},
                

                 // allows full access (via scope) to the api2 resource (which will end up as audience value)
                //new ApiResource("api2", "API2", new[] { "profile", "email" })
                //{
                //    Scopes = { "api2.fullaccess" }
                //},
            };
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                //new Client
                //{
                //    ClientId = "fundraiserApi",
                //    ClientName = "Fundraiser API",
                //    //AllowedGrantTypes = new[] { "urn:ietf:params:oauth:grant-type:token-exchange" },
                //    RequireConsent = false,
                //    ClientSecrets = { new Secret(Environment.GetEnvironmentVariable("JWTSecret").Sha256()) },
                //    AllowedScopes = {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "roles",
                //        "schools"
                //    }
                //},

            new Client
                {
                    AccessTokenLifetime = 1200,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "FundraiserMVC",
                    ClientId = "fundraiserclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    RedirectUris = new List<string>()
                    {
                        Environment.GetEnvironmentVariable("FrontendSettings__ClientUrl") + "signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        Environment.GetEnvironmentVariable("FrontendSettings__ClientUrl") + "signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "schools",
                        "fundraiserapi.fullaccess",
                    },
                    ClientSecrets =
                    {
                        new Secret(Environment.GetEnvironmentVariable("JWTSecret").Sha256()) //TODO: change for sensible value
                    },
            },
                new Client
                {
                   // AccessTokenLifetime = 1200,
                    //AllowOfflineAccess = true,
                    //UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "Fundraiser API - Swagger",
                    ClientId = "fundraiserapi_swagger",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                   // RequireConsent = false,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = {"https://localhost:44307" },
                    RedirectUris = new List<string>()
                    {
                        Environment.GetEnvironmentVariable("FrontendSettings__ApiUrl") + "swagger/oauth2-redirect.html",
                    //    Environment.GetEnvironmentVariable("FrontendSettings__ApiUrl") + "signin-oidc"
                    },
                    AllowedScopes =
                    {   
                       // IdentityServerConstants.StandardScopes.OpenId,
                       // IdentityServerConstants.StandardScopes.Profile,
                        "fundraiserapi.fullaccess",
                    },
                    ClientSecrets =
                    {
                        new Secret(Environment.GetEnvironmentVariable("JWTSecret").Sha256()) //TODO: change for sensible value
                    },
                    //AlwaysIncludeUserClaimsInIdToken = true,
                }
            };
    }
}