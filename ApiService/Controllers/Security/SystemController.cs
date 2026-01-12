using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.AdminEntities.Users;
using Microsoft.AspNetCore.Authorization;
using ApiService.Domain.Security;
using Microsoft.AspNetCore.Mvc;

namespace ApiService.Controllers.Security
{
    [ApiController]
    [Route("[controller]")]
    public class SystemController : Controller
    {
        private readonly IUserAdminService _userAdminService;
        private readonly IAuthorizationGroupService _authorizationGroupService;
        private readonly IClaimContext _claimContext;
        public SystemController(
            IUserAdminService userAdminService,
            IClaimContext claimContext,
            IAuthorizationGroupService authorizationGroupService)
        {
            _userAdminService = userAdminService;
            _claimContext = claimContext;
            _authorizationGroupService = authorizationGroupService;
        }

        [HttpPost("[Action]/v1")]
        [AllowAnonymous]
        public async Task<IActionResult> Define()
        {
            try
            {
                _claimContext.SetClaim();
                await _authorizationGroupService.GenerateDefault();
                await _userAdminService.GenerateDefault();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
