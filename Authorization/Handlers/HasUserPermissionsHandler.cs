using Library_System_API.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Library_System_API.Authorization.Handlers
{
    public class HasUserPermissionsHandler
    : AuthorizationHandler<HasUserPermissionsRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            HasUserPermissionsRequirement requirement)
        {
            // Admin override
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var permissionsClaim = context.User.FindFirstValue("permissions");


            if (!int.TryParse(permissionsClaim, out int userPermissions))
                return Task.CompletedTask;

            if (userPermissions == -1 ||
                (userPermissions & requirement.Permissions) != 0)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
