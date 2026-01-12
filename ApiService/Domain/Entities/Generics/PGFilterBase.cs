using System.Linq.Expressions;

namespace ApiService.Domain.Entities.Generics
{
    public class PGFilterBase<T> where T : PostgresEntity, new()
    {
        private int _take = 10;
        private int _page = 1;
        public int Take
        {
            get => _take;
            set => _take = value > 0 ? value : 10;
        }
        public int Page
        {
            get => _page;
            set => _page = value > 0 ? value : 1;
        }
        public int Skip => (Page - 1) * Take;
        public string? Search { get; set; }

        public IQueryable<T> Apply(IQueryable<T> query) => ApplyOrderBy(ApplyFilter(query));
        public virtual IQueryable<T> ApplyFilter(IQueryable<T> query) => query;
        private IQueryable<T> ApplyOrderBy(IQueryable<T> query)
        {
            if (OrderDesc) return query.OrderByDescending(OrderBy);
            return query.OrderBy(OrderBy);
        }
        public bool OrderDesc { get; set; } = true;
        public Expression<Func<T, object>> OrderBy { get; set; } = x => x.CreateDate;
    }
}
