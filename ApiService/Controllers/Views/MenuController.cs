using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiService.Domain.Views;

namespace ApiService.Controllers.Dashboards
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("[Action]/v1")]
        public async Task<IActionResult> MainMenus()
        {
            try
            {
                var menus = await _menuService.MainMenus();

                return Ok(menus);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[Action]/v1")]
        public async Task<IActionResult> CountMainMenus()
        {
            try
            {
                //var menus = _menuService.CountMainMenus();

                return Ok(new string[0]);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
    
}
