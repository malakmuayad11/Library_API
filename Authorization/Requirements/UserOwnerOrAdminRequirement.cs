using Microsoft.AspNetCore.Authorization;

namespace Library_System_API.Authorization.Requirements
{
    public class UserOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
