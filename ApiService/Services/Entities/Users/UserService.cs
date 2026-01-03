using ApiService.Domain.Entities.Generics;
using ApiService.Domain.Entities.Users;
using ApiService.Models.Lists;
using ApiService.Models.Users;
using Common;

namespace ApiService.Services.Entities.Users
{
    public class UserService : IUserService
    {
        private readonly IGenericPostgreRepository<User> _repository;
        public UserService(IGenericPostgreRepository<User> repository)
        {
            _repository = repository;
        }
        public async Task<PaginationModel<User>> GetAll(UserFilter filter) => await _repository.GetPagedAsync(filter);
        public async Task<User> GetByKey(Guid id) => await _repository.GetByKey(id);
        public async Task Add(UserCreateModel obj)
        {
            await Validate(obj);
            if (obj.Password != obj.ConfirmPassword) throw new SException(Common.Messages.Messages.PasswordsDoNotMatch);
            obj.SetPassword(obj.Password);
            await _repository.Add(obj);
        }
        private async Task Validate(User obj)
        {
            obj.Validate();
            var emailInUser = await _repository.Exist(u => u.Id != obj.Id && u.Email == obj.Email);

            if (emailInUser) throw new SException(Common.Messages.Messages.EmailIsAlreadyInUse, obj.Email);
        }
        public async Task Update(User obj)
        {
            var objInDB = await GetByKey(obj.Id);
            if (objInDB is null) throw new SException(Common.Messages.Messages.NotFound, Common.Messages.Messages.User);
            await Validate(obj);
            await _repository.Update(obj);
        }
    }
}
