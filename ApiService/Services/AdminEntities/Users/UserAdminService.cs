using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.AdminEntities.Users;
using ApiService.Domain.Entities.Generics;
using ApiService.Models.Lists;
using ApiService.Definitions;

namespace ApiService.Services.AdminEntities.Users
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IGenericPostgreRepository<UserAdmin> _userAdminRepository;
        private readonly IAuthorizationGroupRepository _authorizationGroupRepository;
        public UserAdminService(
            IGenericPostgreRepository<UserAdmin> userAdminRepository,
            IAuthorizationGroupRepository authorizationGroupRepository
            )
        {
            _userAdminRepository = userAdminRepository;
            _authorizationGroupRepository = authorizationGroupRepository;
        }
        public async Task GenerateDefault()
        {
            var userAdmin = new UserAdmin
            {
                Id = DefaultValues.AdminTenantId,
                Name = "Inovy",
                Email = "inovy@inovy.com.br",
                TenantId = DefaultValues.AdminTenantId
            };
            var existingUser = await _userAdminRepository.Exist(x => x.Name == userAdmin.Name && x.Email == userAdmin.Email);
            if (existingUser) return;

            userAdmin.AuthorizationGroupId = DefaultValues.AdminTenantId;

            userAdmin.Password = "100000.rb3VMwggfaOR3sZhz68gXQ==.erRFakXGGAym7UC2Uybo+8UuO5ZGofW9QMJgC8j1FUs=";

            await Add(userAdmin);
        }
        public async Task Add(UserAdmin user)
        {
            await _userAdminRepository.Add(user);
        }
        public async Task<PaginationModel<UserAdmin>> GetAll(UserAdminFilter userAdminFilter)
        {
            return await _userAdminRepository.GetPagedAsync(userAdminFilter);
        }
    }
}
