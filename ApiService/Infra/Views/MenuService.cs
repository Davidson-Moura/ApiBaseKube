using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.Security;
using ApiService.Domain.Views;

namespace ApiService.Infra.Views
{
    public class MenuService : IMenuService
    {
        private readonly IClaimContext _claimContext;
        private readonly IAuthorizationGroupCache _authorizationGroupCache;
        public MenuService(
            IAuthorizationGroupCache authorizationGroupCache,
            IClaimContext claimContext)
        {
            _authorizationGroupCache = authorizationGroupCache;
            _claimContext = claimContext;
        }
        public async Task<List<Menu>> MainMenus()
        {
            var group = await _authorizationGroupCache.GetCurrentAuthorizationGroup();
            var menu = new List<Menu>()
                {
                    new Menu()
                    {
                        title = "label.settings",
                        icon = "mdi-cogs",
                        list = new List<Menu>()
                        {
                            new Menu()
                            {
                                key = AuthorizationRoleEnum.U_V.ToString(),
                                title = "label.user",
                                icon = "mdi-account-multiple",
                                route = "Users",
                                sized = 1
                            }
                        }
                    },
                };

            var menus = FilterMenus(menu, group);

            return menu;
        }
        
        private List<Menu> FilterMenus(List<Menu> menu, AuthorizationGroup group)
        {
            var newMenus = new List<Menu>();
            menu.ForEach(m =>
            {
                if (!string.IsNullOrEmpty(m.route))
                {
                    if (group.Roles.Exists(x => x.Role == m.key)) newMenus.Add(m);
                }
                else
                {
                    if (m.list is not null && m.list.Count > 0)
                    {
                        m.list = FilterMenus(m.list, group);
                        if (m.list.Count > 0) newMenus.Add(m);
                    }
                }
            });

            return newMenus;
        }
    }
}