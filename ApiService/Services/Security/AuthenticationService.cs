using ApiService.Domain.Entities.Users;
using ApiService.Domain.Security;
using ApiService.Models.Users;
using Common;

namespace ApiService.Services.Security
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly TokenService _tokenService;
        private readonly IUserService _userService;

        public AuthenticationService(
            TokenService tokenService, 
            IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<UserLoginResponseModel> UserAuthenticate(string login, string password, HttpResponse response)
        {
            var userModel = await _userService.GetAll(new UserFilter()
            {
                EmailEq = login
            });
            if (userModel.Count != 1) throw new SException(Common.Messages.Messages.InvalidCredentials);
            var user = userModel.List.First();
            if (user.Email.ToLower() != login.ToLower()) throw new SException(Common.Messages.Messages.InvalidCredentials);
            if (!user.ValidatePassword(password)) throw new SException(Common.Messages.Messages.InvalidCredentials);

            return _tokenService.GenerateToken(user, response);
        }
    }
}
