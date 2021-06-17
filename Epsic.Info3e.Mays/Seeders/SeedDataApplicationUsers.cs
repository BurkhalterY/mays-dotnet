using System.Linq;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Identity;

namespace Epsic.Info3e.Mays.Seeders
{
    public static class SeedDataApplicationUsers
    {

        private class FakeUser
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Avatar { get; set; } = null;
        }

        public static void SeedUsers(UserManager<User> userManager)
        {
            var admins = new FakeUser[] {
                new FakeUser {
                    Username = "admin",
                    Email = "admin@mays.ch",
                    Password = "Adm1n2345+",
                },
            };
            var premiums = new FakeUser[] {
                new FakeUser {
                    Username = "JefMesos",
                    Email = "jeff@elmesos.mx",
                    Password = "4rg#ent$",
                },
            };
            var users = new FakeUser[] {
                new FakeUser {
                    Username = "bobMichel",
                    Email = "bob.michel@gode.ch",
                    Password = "#ard4nal",
                },
            };

            // Add admins and premiums to users
            users = users.Concat(admins).Concat(premiums).ToArray();

            foreach (var admin in admins)
            {
                var exists = userManager.Users.Any(u => u.UserName == admin.Username);
                if (!exists)
                {

                    var newAdmin = new User {
                        Email = admin.Email,
                        UserName = admin.Username,
                    };

                    userManager.CreateAsync(newAdmin, admin.Password);

                    userManager.AddToRolesAsync(newAdmin, new string[] { "admin" });
                }
            }
            foreach (var premium in premiums)
            {
                var exists = userManager.Users.Any(u => u.UserName == premium.Username);
                if (!exists)
                {

                    var newAdmin = new User {
                        Email = premium.Email,
                        UserName = premium.Username,
                    };

                    userManager.CreateAsync(newAdmin, premium.Password);

                    userManager.AddToRolesAsync(newAdmin, new string[] { "premium" });
                }
            }
            foreach (var user in users)
            {
                var exists = userManager.Users.Any(u => u.UserName == user.Username);
                if (!exists)
                {

                    var newAdmin = new User {
                        Email = user.Email,
                        UserName = user.Username,
                    };

                    userManager.CreateAsync(newAdmin, user.Password);

                    userManager.AddToRolesAsync(newAdmin, new string[] { "user" });
                }
            }
        }
    }
}
