using ApiService.Models.Users;

namespace ApiService.Domain.Security
{
    public interface IAuthenticationService
    {
        Task<UserLoginResponseModel> UserAuthenticate(string login, string password, HttpResponse response);
    }
}
