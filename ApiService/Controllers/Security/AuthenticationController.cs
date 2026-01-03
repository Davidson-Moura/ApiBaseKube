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
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
    }
}
