namespace ApiService.Domain.Views
{
    public interface IMenuService
    {
        Task<List<Menu>> MainMenus();
    }
    public class Menu
    {
        public string title { get; set; }
        public string icon { get; set; }
        public string route { get; set; }
        public bool locked { get; set; }
        public int sized { get; set; }
        public List<Menu> list { get; set; }
        public string key { get; set; }
        public string actionRoute { get; set; }
        public string actionTitle { get; set; }
    }
    public class MenuCount
    {
        public string key { get; set; }
        public long count { get; set; }
    }
}
