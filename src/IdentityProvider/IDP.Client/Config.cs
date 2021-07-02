// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Utils;
using IDPClient = IdentityServer4.Models.Client;

namespace IDP.Client
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(
                    "roles", //scopename
                    "Your role(s)", //display name
                    new List<string> {JwtClaimTypes.Role}), //list of claims
                new IdentityResource(
                    "school", //scopename
                    "Your school and group", //display name
                    new List<string> {CustomClaimTypes.SchoolId, CustomClaimTypes.GroupId}) //list of claims
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("fundraiserapi.fullaccess", "Backend.API")
                //new ApiScope(IdentityServerConstants.LocalApi.ScopeName + ".read", IdentityServerConstants.LocalApi.ScopeName + ".read"),
                //new ApiScope(IdentityServerConstants.LocalApi.ScopeName + ".manage", IdentityServerConstants.LocalApi.ScopeName + ".manage")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                // allows full access (via scope) to the api1 resource (which will end up as audience value)
                new ApiResource("fundraiserapi", "Backend.API",
                    new List<string> {JwtClaimTypes.Role, CustomClaimTypes.SchoolId, CustomClaimTypes.GroupId})
                {
                    Scopes = {"fundraiserapi.fullaccess"} // "roles"
                    //ApiSecrets = { new Secret("apisecret".Sha256()) }
                }

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

        public static IEnumerable<IDPClient> Clients =>
            new[]
            {
                //new Client
                //{
                //    ClientId = "fundraiserApi",
                //    ClientName = "Backend.API",
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

                new IDPClient
                {
                    AccessTokenLifetime = 1200,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "FundraiserMVC",
                    ClientId = "fundraiserclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    RedirectUris = new List<string>
                    {
                        EnvironmentVariables.ClientUrl + "signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        EnvironmentVariables.ClientUrl + "signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "school",
                        "fundraiserapi.fullaccess"
                    },
                    ClientSecrets =
                    {
                        new Secret(EnvironmentVariables.JwtSecret.Sha256())
                    }
                },
                new IDPClient
                {
                    // AccessTokenLifetime = 1200,
                    //AllowOfflineAccess = true,
                    //UpdateAccessTokenClaimsOnRefresh = true,
                    ClientName = "Backend.API - Swagger",
                    ClientId = "fundraiserapi_swagger",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    // RequireConsent = false,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = {"https://localhost:44307"},
                    RedirectUris = new List<string>
                    {
                        EnvironmentVariables.ApiUrl + "swagger/oauth2-redirect.html"
                        //    Environment.GetEnvironmentVariable("FrontendSettings__ApiUrl") + "signin-oidc"
                    },
                    AllowedScopes =
                    {
                        // IdentityServerConstants.StandardScopes.OpenId,
                        // IdentityServerConstants.StandardScopes.Profile,
                        "fundraiserapi.fullaccess"
                    },
                    ClientSecrets =
                    {
                        new Secret(EnvironmentVariables.JwtSecret.Sha256())
                    }
                    //AlwaysIncludeUserClaimsInIdToken = true,
                }
            };
    }
}