using System.Threading.Tasks;
using Epsic.Info3e.Mays.Models;
using Microsoft.AspNetCore.Authorization;

namespace Epsic.Info3e.Mays.Authorization
{
    public class SameUserAuthorizationHandler : AuthorizationHandler<SameUserRequirement, Post>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, Post resource)
        {
            var userId = context.User.FindFirst("Id");
            if (userId.Value == resource.Author?.Id)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class SameUserRequirement : IAuthorizationRequirement { }
}