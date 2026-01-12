using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.Entities.Generics;
using ApiService.Domain.Security;
using System.Linq.Expressions;

namespace ApiService.Infra.Entities.Authorizations.AuthorizationGroups
{
    public class AuthorizationGroupRepository : IAuthorizationGroupRepository
    {
        private readonly IClaimContext _claimContext;
        private readonly IGenericPostgreRepository<AuthorizationGroup> _genericRepository;
        public AuthorizationGroupRepository(
            IGenericPostgreRepository<AuthorizationGroup> genericRepository,
            IClaimContext claimContext)
        {
            _genericRepository = genericRepository;
            _claimContext = claimContext;
        }
        public async Task<AuthorizationGroup> GetById(Guid id) => await _genericRepository.GetByKey(id);
        public async Task<bool> Exist(Expression<Func<AuthorizationGroup, bool>>  exp) => await _genericRepository.Exist(exp);
        public async Task Save(AuthorizationGroup obj)
        {
            obj.TenantId = _claimContext.TenantId;
            obj.Roles.ForEach(r => 
            {
                r.CreateDate = DateTime.Now;
                r.TenantId = _claimContext.TenantId;
            });

            await _genericRepository.Add(obj);
        }
    }
}
