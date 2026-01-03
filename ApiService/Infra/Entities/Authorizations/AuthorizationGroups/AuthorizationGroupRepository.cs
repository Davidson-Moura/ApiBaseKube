using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.Databases;

namespace ApiService.Infra.Entities.Authorizations.AuthorizationGroups
{
    public class AuthorizationGroupRepository : IAuthorizationGroupRepository
    {
        private readonly IConnectionContext _dbContext;
        public AuthorizationGroupRepository(IConnectionContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
