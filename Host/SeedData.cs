// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using Host.Data;
using IdentityModel;
using IdentityServer.ActiveDirectory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Host
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var manuvilachan = userMgr.FindByNameAsync("manuvilachan").Result;
                if (manuvilachan == null)
                {
                    manuvilachan = new ApplicationUser { UserName = "manuvilachan" };
                    var result = userMgr.CreateAsync(manuvilachan, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(manuvilachan, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Manuel Vilachán"),
                        new Claim(JwtClaimTypes.GivenName, "Manuel"),
                        new Claim(JwtClaimTypes.FamilyName, "Vilachan"),
                        new Claim(JwtClaimTypes.Email, "manuel.vilachan@expertscoding.es"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.expertscoding.es"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("manuvilachan created");
                }
                else
                {
                    Console.WriteLine("manuvilachan already exists");
                }

                var bob = userMgr.FindByNameAsync("antoniomarin").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser{UserName = "antoniomarin"};
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Antonio Marín"),
                        new Claim(JwtClaimTypes.GivenName, "Antonio"),
                        new Claim(JwtClaimTypes.FamilyName, "Marín"),
                        new Claim(JwtClaimTypes.Email, "antonio.marin@expertscoding.es"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://www.expertscoding.es"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("antoniomarin created");
                }
                else
                {
                    Console.WriteLine("antoniomarin already exists");
                }
            }
        }
    }
}
