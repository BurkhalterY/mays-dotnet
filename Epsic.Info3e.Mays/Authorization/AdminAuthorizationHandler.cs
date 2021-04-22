using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Epsic.Info3e.Mays.Authorization
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (context.User.IsInRole("admin"))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class AdminRequirement : IAuthorizationRequirement { }
}