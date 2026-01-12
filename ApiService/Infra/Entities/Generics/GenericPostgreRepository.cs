using ApiService.Definitions;
using ApiService.Domain.Databases;
using ApiService.Domain.Entities;
using ApiService.Domain.Entities.Generics;
using ApiService.Domain.Security;
using ApiService.Models.Lists;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiService.Infra.Entities.Generics
{
    public class GenericPostgreRepository<T> : IGenericPostgreRepository<T> where T : PostgresEntity, new()
    {
        private readonly IConnectionContext _connectionContext;
        private readonly IClaimContext _claimContext;
        public GenericPostgreRepository(
            IConnectionContext connectionContext, 
            IClaimContext claimContext)
        {
            _connectionContext = connectionContext;
            _claimContext = claimContext;
        }
        private void VerifyPermission()
        {
            if (_claimContext.TenantId == Guid.Empty) throw new UnauthorizedAccessException();
        }
        private async Task VerifyTenant(T obj)
        {
            await VerifyTenant(obj.Id);
        }
        private async Task VerifyTenant(Guid id)
        {
            var isCurrentTenant = await Exist(x => x.Id == id && x.TenantId == _claimContext.TenantId);
            if (!isCurrentTenant) throw new UnauthorizedAccessException();
        }
        public async Task<PaginationModel<T>> GetPagedAsync(PGFilterBase<T> filter)
        {
            VerifyPermission();
            var query = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();

            var filteredQuery = filter.ApplyFilter(query);

            if (_claimContext.TenantId != DefaultValues.AdminTenantId)
                filteredQuery = filteredQuery.Where(x => x.TenantId == _claimContext.TenantId);

            var total = await filteredQuery.CountAsync();

            var items = await
                filteredQuery
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
            VerifyPermission();
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();
            var obj = await dataSet.FirstOrDefaultAsync(x => x.TenantId == _claimContext.TenantId && x.Id == id);
            return obj;
        }
        public async Task<T2> GetProperty<T2>(Guid id, Expression<Func<T, T2>> exp)
        {
            VerifyPermission();
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();
            return await dataSet.Where(x => x.TenantId == _claimContext.TenantId && x.Id == id).Select(exp).FirstOrDefaultAsync();
        }
        public async Task<bool> Exist(Expression<Func<T, bool>> exp)
        {
            VerifyPermission();
            exp = SetTenantFilter(exp);

            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>().AsNoTracking();
            return await dataSet.AnyAsync(exp);
        }
        private Expression<Func<T, bool>> SetTenantFilter(Expression<Func<T, bool>> exp)
        {
            return Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(
                        exp.Body,
                        Expression.Equal(
                            Expression.Property(exp.Parameters[0], nameof(PostgresEntity.TenantId)),
                            Expression.Constant(_claimContext.TenantId)
                        )
                    ),
                    exp.Parameters
                );
        }
        public async Task Add(T obj)
        {
            VerifyPermission();

            obj.GenerateId();
            obj.Validate();
            obj.CreateDate = DateTime.UtcNow;
            obj.UpdateDate = DateTime.MinValue;
            obj.TenantId = _claimContext.TenantId;

            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            await dataSet.AddAsync(obj);
            _connectionContext.PostgreConnection.MarkChanges();
        }
        public async Task Update(T obj)
        {
            VerifyPermission();
            await VerifyTenant(obj);

            obj.UpdateDate = DateTime.UtcNow;
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            dataSet.Update(obj);
            _connectionContext.PostgreConnection.MarkChanges();
        }
        public async Task Delete(T obj)
        {
            VerifyPermission();
            await VerifyTenant(obj);
            await Delete(obj.Id);
        }
        public async Task Delete(Guid id)
        {
            VerifyPermission();
            await VerifyTenant(id);
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            await dataSet.Where(x => x.Id == id).ExecuteDeleteAsync();
            _connectionContext.PostgreConnection.MarkChanges();
        }
        public async Task Delete(IEnumerable<T> objs)
        {
            VerifyPermission();
            var ids = objs.Select(x => x.Id).ToList();
            await Delete(ids);
        }
        public async Task Delete(List<Guid> ids)
        {
            VerifyPermission();
            var dataSet = _connectionContext.PostgreConnection.GetEntity<T>();
            await dataSet.Where(x => ids.Contains(x.Id) && x.TenantId == _claimContext.TenantId).ExecuteDeleteAsync();
            _connectionContext.PostgreConnection.MarkChanges();
        }
    }
}
