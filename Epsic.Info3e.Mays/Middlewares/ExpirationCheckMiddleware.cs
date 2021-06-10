using System;
using System.Linq;
using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Epsic.Info3e.Mays.Middlewares
{
    public class ExpirationCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public ExpirationCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<User> userManager)
        {
            if (context.User != null && context.User.Claims.Any(c => c.Type == "id"))
            {
                var user = await userManager.FindByIdAsync(context.User.Claims.First(c => c.Type == "id").Value);

                if (user.ExpirationDate > DateTime.Now)
                {
                    if (user.AutoRenew)
                    {
                        user.ExpirationDate = DateTime.Now.AddMonths(1);
                        // Untested but probably works
                        await userManager.UpdateAsync(user);
                    }
                    else
                    {
                        await userManager.RemoveFromRoleAsync(user, "premium");
                    }
                }
            }

            await _next.Invoke(context);
        }
    }

    public static class ExpirationCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseExpirationCheck(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExpirationCheckMiddleware>();
        }
    }
}
