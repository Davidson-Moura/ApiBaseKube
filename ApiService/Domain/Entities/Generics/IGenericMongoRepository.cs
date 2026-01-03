using ApiService.Infra.Entities.Generics;
using ApiService.Models.Lists;

namespace ApiService.Domain.Entities.Generics
{
    public interface IGenericMongoRepository<T> where T : MongoEntity, new() 
    {
        Task Add(T obj);
        Task Add(IEnumerable<T> objs);
        Task<T> Get(Guid id);
        Task<PaginationModel<T>> GetAll(MGFilterBase<T> filter);
        Task Update(T obj);
        Task Delete(T obj);
        Task Delete(Guid id);
        Task Delete(IEnumerable<Guid> ids);
    }
}
