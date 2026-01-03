using ApiService.Domain.Entities.Generics;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Databases;
using ApiService.Domain.Entities;
using ApiService.Models.Lists;
using System.Linq.Expressions;

namespace ApiService.Infra.Entities.Generics
{
    public class GenericPostgreRepository<T> : IGenericPostgreRepository<T> where T : PostegresEntity, new()
    {
        private readonly IConnectionContext _connectionContext;
        public GenericPostgreRepository(
            IConnectionContext connectionContext)
        {
            _connectionContext = connectionContext;
        }
        public async Task<PaginationModel<T>> GetPagedAsync(PGFilterBase<T> filter)
        {
            var query = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();

            var total = await filter.Apply(query).CountAsync();

            var items = await
                filter.Apply(query)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToListAsync();

            var pages = (int)Math.Ceiling((double)total / filter.Take);

            return new PaginationModel<T>
            {
                List = items,
                Count = total,
                Pages = pages
            };
        }
        public async Task<T> GetByKey(Guid id)
        {
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();
            var obj = await dataSet.FirstOrDefaultAsync(x => x.Id == id);
            return obj;
        }
        public async Task<T2> GetProperty<T2>(Guid id, Expression<Func<T, T2>> exp)
        {
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();
            return await dataSet.Where(x => x.Id == id).Select(exp).FirstOrDefaultAsync();
        }
        public async Task<bool> Exist(Expression<Func<T, bool>> exp)
        {
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();
            return await dataSet.AnyAsync(exp);
        }
        public async Task Add(T obj)
        {
            obj.GenerateId();
            obj.Validate();
            obj.CreateDate = DateTime.UtcNow;
            obj.UpdateDate = DateTime.MinValue;
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            await dataSet.AddAsync(obj);
            _connectionContext.PostgreConnection.MarkChanges();
        }
        public async Task Update(T obj)
        {
            obj.UpdateDate = DateTime.UtcNow;
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            dataSet.Update(obj);
            _connectionContext.PostgreConnection.MarkChanges();
        }
        public async Task Delete(T obj)
        {
            await Delete(obj.Id);
        }
        public async Task Delete(Guid id)
        {
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            await dataSet.Where(x => x.Id == id).ExecuteDeleteAsync();
            _connectionContext.PostgreConnection.MarkChanges();
        }
        public async Task Delete(IEnumerable<T> objs)
        {
            var ids = objs.Select(x => x.Id).ToList();
            await Delete(ids);
        }
        public async Task Delete(List<Guid> ids)
        {
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            await dataSet.Where(x => ids.Contains(x.Id) ).ExecuteDeleteAsync();
            _connectionContext.PostgreConnection.MarkChanges();
        }
    }
}
