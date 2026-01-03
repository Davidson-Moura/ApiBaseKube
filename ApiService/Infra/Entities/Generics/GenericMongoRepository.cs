using ApiService.Domain.Entities.Generics;
using ApiService.Domain.Databases;
using ApiService.Domain.Entities;
using ApiService.Models.Lists;

namespace ApiService.Infra.Entities.Generics
{
    public class GenericMongoRepository<T> : IGenericMongoRepository<T> where T : MongoEntity, new()
    {
        private readonly IConnectionContext _dbContext;
        public GenericMongoRepository(IConnectionContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(T obj) 
        {
            obj.Validate();
            await _dbContext.MongoDBConnection.InsertAsync(obj);
        }
        public async Task Add(IEnumerable<T> objs)
        {
            foreach (T obj in objs) obj.Validate();
            await _dbContext.MongoDBConnection.InsertManyAsync(objs);
        }
        public async Task<T> Get(Guid id)
        {
            return await _dbContext.MongoDBConnection.GetByIdAsync<T>(id);
        }
        public async Task<PaginationModel<T>> GetAll(MGFilterBase<T> filter)
        {
            return await _dbContext.MongoDBConnection.GetAllAsync<T>(
                filter.GetFilter(),
                filter.Sort,
                filter.SortDesc,
                filter.Skip,
                filter.Take
                );
        }
        public async Task Update(T obj)
        {
            await _dbContext.MongoDBConnection.UpdateAsync(obj);
        }
        public async Task Delete(T obj)
        {
            await Delete(obj.Id);
        }
        public async Task Delete(Guid id)
        {
            await _dbContext.MongoDBConnection.DeleteAsync<T>(id);
        }
        public async Task Delete(IEnumerable<Guid> ids)
        {
            await _dbContext.MongoDBConnection.DeleteAsync<T>(ids);
        }
    }
}
