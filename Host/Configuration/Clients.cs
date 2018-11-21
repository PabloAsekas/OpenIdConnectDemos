// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Host.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ClientECApi",
                    ClientName = "EC Films API",
                    ClientUri = "http://www.expertscoding.es",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {new Secret("CliEnt.ExpertsCoding.SecReT".Sha256()){Type="SharedSecret"}},

                    AllowedScopes = { "ECApi" }
                },

                ///////////////////////////////////////////
                // MVC Hybrid Flow Samples
                //////////////////////////////////////////
                new Client
                {
                    ClientId = "ECApp",
                    ClientName = "EC Application",
                    ClientUri = "http://www.expertscoding.es",

                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },
                    
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = false,

                    RedirectUris = { "http://localhost:21402/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:21402/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:21402/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "ECApi"
                    }
                }
            };
        }
    }
}