using ApiService.Domain.AdminEntities.Users;
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
        private readonly IUserAdminService _userAdminService;
        private readonly IClaimContext _claimContext;

        public AuthenticationService(
            TokenService tokenService,
            IUserService userService,
            IUserAdminService userAdminService,
            IClaimContext claimContext)
        {
            _tokenService = tokenService;
            _userService = userService;
            _userAdminService = userAdminService;
            _claimContext = claimContext;
        }

        public async Task<UserLoginResponseModel> UserAuthenticate(string login, string password, HttpResponse response)
        {
            _claimContext.SetClaim();
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
        public async Task<UserLoginResponseModel> UserAdminAuthenticate(string login, string password, HttpResponse response)
        {
            _claimContext.SetClaim();
            var userModel = await _userAdminService.GetAll(new UserAdminFilter()
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
