using Microsoft.AspNetCore.Authorization;
using ApiService.Domain.Entities.Users;
using ApiService.Domain.Security;
using Microsoft.AspNetCore.Mvc;
using ApiService.Models.Users;

namespace ApiService.Controllers.Entities.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Route("v1")]
        [Authorize(Policy = nameof(AuthorizationRoleEnum.U_V))]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1, int take = 10, string? search = null
            )
        {
            try
            {
                var filter = new UserFilter()
                {
                    Page = page,
                    Take = take,
                    Search = search
                };

                var model = await _userService.GetAll(filter);
                foreach (var u in model.List) u.HiddenPassword();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("{id:guid}/v1")]
        [Authorize(Policy = nameof(AuthorizationRoleEnum.U_V))]
        public async Task<IActionResult> GetByKey(Guid id)
        {
            try
            {
                var obj = await _userService.GetByKey(id);
                obj.HiddenPassword();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("v1")]
        [Authorize(Policy = nameof(AuthorizationRoleEnum.U_C))]
        public async Task<IActionResult> Add(UserCreateModel user)
        {
            try
            {
                await _userService.Add(user);

                return Created(user.Id.ToString(), user.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPatch]
        [Route("v1")]
        [Authorize(Policy = nameof(AuthorizationRoleEnum.U_U))]
        public async Task<IActionResult> Update(User user)
        {
            try
            {
                await _userService.Update(user);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
