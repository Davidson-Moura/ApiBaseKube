using ApiService.Models.Lists;
using System.Linq.Expressions;

namespace ApiService.Domain.Entities.Generics
{
    public interface IGenericPostgreRepository<T> where T : PostgresEntity, new()
    {
        Task<PaginationModel<T>> GetPagedAsync(PGFilterBase<T> filter);
        Task<T> GetByKey(Guid id);
        Task<T2> GetProperty<T2>(Guid id, Expression<Func<T, T2>> exp);
        Task<bool> Exist(Expression<Func<T, bool>> exp);
        Task Add(T obj);
        Task Update(T obj);
        Task Delete(T obj);
        Task Delete(Guid id);
        Task Delete(IEnumerable<T> objs);
        Task Delete(List<Guid> ids);
    }
}
