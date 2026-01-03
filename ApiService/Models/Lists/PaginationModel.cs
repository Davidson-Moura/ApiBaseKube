namespace ApiService.Models.Lists
{
    public class PaginationModel<T>
    {
        public int Count { get; set; }
        public int Pages { get; set; }
        public IEnumerable<T> List { get; set; } = Enumerable.Empty<T>();
    }
}
