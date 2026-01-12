using System.Linq.Expressions;

namespace ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups
{
    public interface IAuthorizationGroupRepository
    {
        Task<bool> Exist(Expression<Func<AuthorizationGroup, bool>> exp);
        Task<AuthorizationGroup> GetById(Guid id);
        Task Save(AuthorizationGroup obj);
    }
}
