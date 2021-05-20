using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Epsic.Info3e.Mays.Authorization
{
    public class ExtensionAuthorizationHandler : AuthorizationHandler<ExtensionRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExtensionRequirement requirement, string extension)
        {
            var freeExtension = new string[] { "png", "jpg", "jpeg", "gif", "bmp", "webp" }.ToList();
            var premiumExtension = new string[] { "mp4", "webm", "mp3", "wav" }.ToList();

            if (context.User.IsInRole("user"))
            {
                if (freeExtension.Contains(extension))
                {
                    context.Succeed(requirement);
                }
                else if (context.User.IsInRole("premium") || context.User.IsInRole("admin"))
                {
                    if (premiumExtension.Contains(extension))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }

    public class ExtensionRequirement : IAuthorizationRequirement { }
}
