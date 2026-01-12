using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using Microsoft.AspNetCore.Authorization;

namespace ApiService.Domain.Security
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IAuthorizationGroupCache _authorizationGroupCache;
        public PermissionHandler(IAuthorizationGroupCache authorizationGroupCache)
        {
            _authorizationGroupCache = authorizationGroupCache;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var group = await _authorizationGroupCache.GetCurrentAuthorizationGroup();

            if (group.Roles.Exists(x => x.Role == requirement.Permission)) context.Succeed(requirement);
            else context.Fail();
        }
    }
}
