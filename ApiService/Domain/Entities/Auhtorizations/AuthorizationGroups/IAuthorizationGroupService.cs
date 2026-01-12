namespace ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups
{
    public interface IAuthorizationGroupService
    {
        Task<AuthorizationGroup> GetById(Guid id);
        Task Save(AuthorizationGroup obj);
        Task GenerateDefault();
    }
}
