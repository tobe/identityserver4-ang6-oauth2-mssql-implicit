// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using AuthServer.Data;
using AuthServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace AuthServer
{
    public class SeedData
    {
        public static async Task EnsureSeedDataAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                // This will run the migrations if they already haven't been run, might be a good idea to remove this!
                context.Database.Migrate();

                // Get the role manager from DI, and check whether an admin role is present, if not create a new one
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var adminRole = await roleManager.RoleExistsAsync("Administrator");
                if (!adminRole)
                    await roleManager.CreateAsync(new IdentityRole("Administrator"));

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var admin = userMgr.FindByNameAsync("admin").Result;
                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "admin"
                    };
                    var result = userMgr.CreateAsync(admin, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    // Add the admin user to the Administrator role
                    await userMgr.AddToRoleAsync(admin, "Administrator");

                    result = userMgr.AddClaimsAsync(admin, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Administrator"),
                        new Claim(JwtClaimTypes.Email, "administrator@localhost.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "https://example.org"),
                    }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("admin created");
                }
                else
                {
                    Console.WriteLine("admin already exists");
                }
            }
        }
    }
}
