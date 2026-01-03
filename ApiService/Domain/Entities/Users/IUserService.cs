using ApiService.Models.Lists;
using ApiService.Models.Users;

namespace ApiService.Domain.Entities.Users
{
    public interface IUserService
    {
        Task<PaginationModel<User>> GetAll(UserFilter filter);
        Task<User> GetByKey(Guid id);
        Task Add(UserCreateModel obj);
        Task Update(User obj);
    }
}
