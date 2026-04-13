using Microsoft.AspNetCore.Authorization;

namespace Library_System_API.Authorization.Requirements
{
    public class HasUserPermissionsRequirement : IAuthorizationRequirement
    {
        public int Permissions { get; }

        public HasUserPermissionsRequirement(int Permissions) =>
            this.Permissions = Permissions;
    }
}
