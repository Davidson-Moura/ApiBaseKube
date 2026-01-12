using ApiService.Definitions;
using ApiService.Domain.Caches;
using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.Security;
using Common;
using Common.Messages;

namespace ApiService.Infra.Entities.Authorizations.AuthorizationGroups
{
    public class AuthorizationGroupCache : IAuthorizationGroupCache
    {
        private readonly ICacheService _cacheService;
        private readonly IClaimContext _claimContext;
        private readonly IAuthorizationGroupRepository _authorizationGroupRepository;
        public AuthorizationGroupCache(ICacheService cacheService,
            IAuthorizationGroupRepository authorizationGroupRepository,
            IClaimContext claimContext)
        {
            _cacheService = cacheService;
            _authorizationGroupRepository = authorizationGroupRepository;
            _claimContext = claimContext;
        }
        public async Task<AuthorizationGroup> GetCurrentAuthorizationGroup()
        {
            if (_claimContext.AuthorizationGroupId is null || _claimContext.AuthorizationGroupId == Guid.Empty) throw new SException(Messages.AuthorizationGroupNotFound);
            return await GetByIdInCache(_claimContext.AuthorizationGroupId.ToString());
        }
        public async Task<List<AuthorizationGroupRole>> GetCurrentRoles()
        {
            var gp = await GetByIdInCache(_claimContext.AuthorizationGroupId.ToString());
            return gp.Roles;
        }
        public async Task<AuthorizationGroup> GetByIdInCache(string authorizationGroupId)
        {
            if (string.IsNullOrEmpty(authorizationGroupId)) throw new SException(Messages.AuthorizationGroupNotFound);
            if (_claimContext.TenantId == Guid.Empty) throw new SException(Messages.AuthorizationGroupNotFound);

            var key = _cacheService.GetKey(CachePrefix.auth_group, _claimContext.TenantId.ToString(), authorizationGroupId);

            var authGroup = _cacheService.GetAsync<AuthorizationGroup>(key).Result;

            if (authGroup is null)
            {
                authGroup = await _authorizationGroupRepository.GetById(new Guid(authorizationGroupId));
                if (authGroup.IsSystem)
                {
                    authGroup.Roles = new List<AuthorizationGroupRole>();

                    foreach (var e in System.Enum.GetValues(typeof(AuthorizationRoleEnum))
                    .Cast<AuthorizationRoleEnum>()
                    .Select(x => x.ToString()).ToList())
                    {
                        authGroup.Roles.Add(new AuthorizationGroupRole()
                        {
                            Name = SMessage.Message(e),
                            Role = e,
                            TenantId = authGroup.TenantId
                        });
                    };
                }
                //var enterpriser = _enterpriserCacheService.GetCurrent();

                //authGroup.SetFilteredRoles(enterpriser.AuthorizationPrefixs);
                _cacheService.SetAsync(key, authGroup);
            }
            if (authGroup is null) throw new SException(Common.Messages.Messages.AuthorizationGroupNotFound);

            return authGroup;
        }
        public void SetAuthorizationGroup(AuthorizationGroup gp)
        {
            if (_claimContext.TenantId == Guid.Empty) throw new SException(Messages.AuthorizationGroupNotFound);
            _cacheService.SetAsync(_cacheService.GetKey(CachePrefix.auth_group, _claimContext.TenantId.ToString(), gp.Id.ToString()), gp);
        }
        public async Task<bool> CanAction(AuthorizationRoleEnum permission)
        {
            return (await GetCurrentRoles()).Exists(x => x.Role == permission.ToString());
        }
    }
}
