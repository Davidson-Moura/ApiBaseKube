using ApiService.Models.Lists;

namespace ApiService.Domain.AdminEntities.Users
{
    public interface IUserAdminService
    {
        Task GenerateDefault();
        Task<PaginationModel<UserAdmin>> GetAll(UserAdminFilter userAdminFilter);
        Task Add(UserAdmin user);
    }
}
