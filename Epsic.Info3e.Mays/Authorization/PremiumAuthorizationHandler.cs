using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Epsic.Info3e.Mays.Authorization
{
    public class PremiumAuthorizationHandler : AuthorizationHandler<PremiumRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PremiumRequirement requirement)
        {
            if (context.User.IsInRole("premium") || context.User.IsInRole("admin"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class PremiumRequirement : IAuthorizationRequirement { }
}