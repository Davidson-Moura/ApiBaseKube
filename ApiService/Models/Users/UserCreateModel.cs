using ApiService.Domain.Entities.Users;

namespace ApiService.Models.Users
{
    public class UserCreateModel : User
    {
        public string Password { get => base.Password; set => base.Password = value; }
        public string ConfirmPassword { get; set; }
    }
}
