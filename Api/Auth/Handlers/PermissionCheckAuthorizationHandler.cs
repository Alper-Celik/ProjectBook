using System.Security.Claims;

using Api.Auth.Models;

using Microsoft.AspNetCore.Authorization;

namespace Api.Auth.Handlers;

class PermissionCheckAuthorizationHandler : AuthorizationHandler<PermissionCheckAuthorizeAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionCheckAuthorizeAttribute requirement)
    {
        var userPermissionBitsString = context.User.FindFirstValue(UserToken.PermissionBitsType);
        if (!long.TryParse(userPermissionBitsString, out long userPermissionBitsLong))
        {
            return Task.CompletedTask;
        }
        var userPermissionBits = (UserPermissionBits)userPermissionBitsLong;

        if ((requirement.PermissionBits & userPermissionBits) == requirement.PermissionBits)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}

class PermissionCheckAuthorizeAttribute(UserPermissionBits permissionBits) : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public UserPermissionBits PermissionBits { get; set; } = permissionBits;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}