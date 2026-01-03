using ApiService.Domain.Entities.Users;
using Microsoft.AspNetCore.Mvc;
using ApiService.Models.Users;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Add(Guid id)
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
        [AllowAnonymous]
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
