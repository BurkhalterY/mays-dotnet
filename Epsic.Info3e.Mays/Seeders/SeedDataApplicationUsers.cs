﻿using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Epsic.Info3e.Mays.Seeders
{
    public static class SeedDataApplicationUsers
    {
        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            var admins = new[] {
                new {
                    Username = "admin",
                    Email = "admin@mays.ch",
                    Password = "Adm1n2345+",
                },
            };

            foreach (var admin in admins)
            {
                var exists = userManager.Users.Any(u => u.UserName == admin.Username);
                if (!exists)
                {

                    var newAdmin = new IdentityUser {
                        Email = admin.Email,
                        UserName = admin.Username,
                    };

                    userManager.CreateAsync(newAdmin, admin.Password);

                    userManager.AddToRolesAsync(newAdmin, new string[] { "user", "admin" });
                }
            }
        }
    }
}
