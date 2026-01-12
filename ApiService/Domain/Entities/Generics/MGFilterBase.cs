using ApiService.Domain.Entities;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace ApiService.Domain.Entities.Generics
{
    public abstract class MGFilterBase<T> where T : MongoEntity, new()
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
        public abstract FilterDefinition<T> GetFilter();
        public bool SortDesc { get; set; } = false;
        public Expression<Func<T, object>>? Sort { get; }
    }
}
