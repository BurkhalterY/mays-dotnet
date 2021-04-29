using Microsoft.AspNetCore.Identity;

namespace Epsic.Info3e.Mays.Seeders
{
    public static class SeedDataApplicationRoles
    {
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in new string[] { "user", "premium", "admin" })
            {
                var result = roleManager.RoleExistsAsync(role).Result;
                if (!result)
                {
                    roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
