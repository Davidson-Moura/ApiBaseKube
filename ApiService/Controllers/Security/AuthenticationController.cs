using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using Microsoft.AspNetCore.Authorization;
using ApiService.Models.Responses;
using ApiService.Domain.Security;
using Microsoft.AspNetCore.Mvc;
using ApiService.Models.Users;
using ApiService.Extensions;
using Common.Messages;
using Common;

namespace ApiService.Controllers.Security
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationGroupCache _authorizationGroupCache;
        private readonly IClaimContext _claimContext;
        public AuthenticationController(
            IAuthenticationService authenticationService,
            IAuthorizationGroupCache authorizationGroupCache,
            IClaimContext claimContext)
        {
            _authenticationService = authenticationService;
            _authorizationGroupCache = authorizationGroupCache;
            _claimContext = claimContext;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("[Action]/v1")]
        public async Task<IActionResult> Login(UserLoginModel userLogin)
        {
            try
            {
                if (userLogin is null)
                    throw new SException(Messages.InvalidData);
                if (string.IsNullOrEmpty(userLogin.Login) || string.IsNullOrEmpty(userLogin.Password))
                    throw new SException(Messages.InvalidLogin);

                var token = await _authenticationService.UserAuthenticate(userLogin.Login, userLogin.Password, Response);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("[Action]/v1")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                              
                return Ok(
                    new MessageResponse (){ Code = 0, Description = SMessage.Message(Messages.OperationSuccessfully) }
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToReponse());
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[Action]/v1")]
        public async Task<IActionResult> AdminLogin(UserLoginModel userLogin)
        {
            try
            {
                if (userLogin is null)
                    throw new SException(Messages.InvalidData);
                if (string.IsNullOrEmpty(userLogin.Login) || string.IsNullOrEmpty(userLogin.Password))
                    throw new SException(Messages.InvalidLogin);

                var token = await _authenticationService.UserAdminAuthenticate(userLogin.Login, userLogin.Password, Response);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("[Action]/v1")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MyPermissions()
        {
            try
            {
                var group = await _authorizationGroupCache.GetCurrentAuthorizationGroup();
                return Ok(group?.Roles.Select(x => x.Role) ?? new List<string>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
