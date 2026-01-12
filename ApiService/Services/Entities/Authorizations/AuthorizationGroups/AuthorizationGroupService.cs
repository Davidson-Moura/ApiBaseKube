using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.Security;
using ApiService.Definitions;
using Common.Messages;

namespace ApiService.Services.Entities.Authorizations.AuthorizationGroups
{
    public class AuthorizationGroupService : IAuthorizationGroupService
    {
        private readonly IAuthorizationGroupRepository _authorizationGroupRepository;
        public AuthorizationGroupService(IAuthorizationGroupRepository authorizationGroupRepository)
        {
            _authorizationGroupRepository = authorizationGroupRepository;
        }
        public async Task<AuthorizationGroup> GetById(Guid id) => await _authorizationGroupRepository.GetById(id);
        public async Task Save(AuthorizationGroup obj)
        {
            obj.IsSystem = false;
            await _authorizationGroupRepository.Save(obj);
        }
        public async Task GenerateDefault()
        {
            var existing = await _authorizationGroupRepository.Exist(x => x.IsSystem);
            if (existing) return;

            var obj = new AuthorizationGroup()
            {
                Id = DefaultValues.AdminTenantId,
                IsSystem = true,
                Name = "Admin"
            };

            foreach (var e in System.Enum.GetValues(typeof(AuthorizationRoleEnum))
                .Cast<AuthorizationRoleEnum>()
                .Select(x => x.ToString()).ToList())
            {
                obj.Roles.Add(new AuthorizationGroupRole()
                {
                    Name = SMessage.Message(e),
                    Role = e,
                    TenantId = DefaultValues.AdminTenantId
                });
            }
            
            await _authorizationGroupRepository.Save(obj);
        }
    }
}
