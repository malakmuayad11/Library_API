using Microsoft.AspNetCore.Authorization;

namespace Library_System_API.Authorization
{
    public class UserOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
