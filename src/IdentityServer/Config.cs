// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer {
    public static class Config {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[] {
                new IdentityResources.OpenId (),
                new IdentityResources.Profile (),
                new IdentityResources.Email (),
                // new IdentityResource {
                // Name = "custom",
                // UserClaims = new List<string> { "role", "test" },
                // }
            };

        public static IEnumerable<ApiResource> GetApiResources () {
            return new [] {
                new ApiResource {
                    Name = "api1",
                        DisplayName = "API #1",
                        Description = "Allow the application to access API #1 on your behalf",
                        Scopes = new List<string> { "api1.read", "api1.write" },
                        ApiSecrets = new List<Secret> { new Secret ("ScopeSecret".Sha256 ()) }, // change me!
                        UserClaims = new List<string> { "role", "test","website" },
                
                        }
            };
        }

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[] {
                new ApiScope ("api1.read", "Read Access to API #1"),
                new ApiScope ("api1.write", "Write Access to API #1")
            };

        public static IEnumerable<Client> Clients =>
            new Client[] {
                new Client {
                ClientId = "client",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets = {
                new Secret ("secret".Sha256 ())
                },

                // scopes that client has access to
                AllowedScopes = { "api1.read" }
                },

                // interactive ASP.NET Core MVC client
                new Client {
                ClientId = "mvc",
                ClientSecrets = { new Secret ("secret".Sha256 ()) },

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                // where to redirect to after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },
                RequirePkce = false,

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowedScopes = new List<string> {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.Address,

                "api1.read",
                }
                }

                ,
                // interactive ASP.NET Core MVC client
                new Client {
                ClientId = "angular",
                ClientSecrets = { new Secret ("secret".Sha256 ()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect to after login
                RedirectUris = { "https://localhost:5005/Identity/Token" },
                RequirePkce = false,

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:5005/signout-callback-oidc" },

                AllowedScopes = new List<string> {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email
                }
                }
            };
    }
}