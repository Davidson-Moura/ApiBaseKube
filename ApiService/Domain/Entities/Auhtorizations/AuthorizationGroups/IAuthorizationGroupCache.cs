using ApiService.Domain.Security;

namespace ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups
{
    public interface IAuthorizationGroupCache
    {
        Task<AuthorizationGroup> GetCurrentAuthorizationGroup();
        Task<List<AuthorizationGroupRole>> GetCurrentRoles();
        Task<AuthorizationGroup> GetByIdInCache(string authorizationGroupId);
        void SetAuthorizationGroup(AuthorizationGroup gp);
        Task<bool> CanAction(AuthorizationRoleEnum permission);
    }
}
