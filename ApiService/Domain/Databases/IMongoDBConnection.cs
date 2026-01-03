using ApiService.Domain.Entities;
using ApiService.Models.Lists;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ApiService.Domain.Databases
{
    public interface IMongoDBConnection
    {
        void Connect();
        IMongoDatabase MongoDatabase { get; }
        IMongoCollection<T> GetCollection<T>() where T : class, new();
        void Insert<T>(T record) where T : MongoEntity, new ();
        Task InsertAsync<T>(T record) where T : MongoEntity, new();
        void InsertMany<T>(IEnumerable<T> records) where T : MongoEntity, new();
        Task InsertManyAsync<T>(IEnumerable<T> records) where T : MongoEntity, new();
        List<T> GetAll<T>() where T : MongoEntity, new();
        T? GetById<T>(Guid id) where T : MongoEntity, new();
        Task<T> GetByIdAsync<T>(Guid id) where T : MongoEntity, new();
        IEnumerable<T> GetByFilter<T>(Expression<Func<T, bool>> expression) where T : MongoEntity, new();
        //IEnumerable<MongoLog<T1>> GetEntityLogByFilter<T1>(FilterDefinition<MongoLog<T1>> filter) where T1 : IMongoDocument, IMongoLog<T1>, new();
        IEnumerable<T> GetByFilter<T>(FilterDefinition<T> filter) where T : MongoEntity, new();
        IEnumerable<T> GetByFilterOrderByDesc<T>(string propertyName, Expression<Func<T, bool>> expression) where T : MongoEntity, new();
        IEnumerable<T> GetByFilterOrderByDesc<T>(string propertyName, FilterDefinition<T> filter) where T : MongoEntity, new();
        IEnumerable<T> GetByFilterOrderByAsc<T>(string propertyName, Expression<Func<T, bool>> expression) where T : MongoEntity, new();
        //void InsertLogAsync<T>(DocumentMongoLog<T> record) where T : MongoEntity, new();
        //IEnumerable<DocumentMongoLog<T>> GetEntityLogByFilter<T>(FilterDefinition<DocumentMongoLog<T>> filter) where T : MongoEntity, new();
        //long CountEntityLogByFilter<T>(FilterDefinition<DocumentMongoLog<T>> filter) where T : MongoEntity, new();
        long GetCount<T>(Expression<Func<T, bool>> expression) where T : MongoEntity, new();
        long GetCount<T>(FilterDefinition<T> filter) where T : MongoEntity, new();
        long GetEstimedCount<T>() where T : MongoEntity, new();
        void Update<T>(T record) where T : MongoEntity, new();
        Task UpdateAsync<T>(T record) where T : MongoEntity, new();
        bool Exists<T>(Expression<Func<T, bool>> expression) where T : MongoEntity, new();
        bool Any<T>() where T : MongoEntity, new();
        void Delete<T>(Guid id) where T : MongoEntity, new();
        void Delete<T>(IEnumerable<Guid> ids) where T : MongoEntity, new();
        Task DeleteAsync<T>(Guid id) where T : MongoEntity, new();
        Task DeleteAsync<T>(IEnumerable<Guid> ids) where T : MongoEntity, new();

        T GetLast<T>() where T : MongoEntity, new();
        T2 GetProperty<T, T2>(Guid id, Expression<Func<T, T2>> exp) where T : MongoEntity, new();
        T2 GetPropertyBy<T, T2>(Expression<Func<T, bool>> where, Expression<Func<T, T2>> prop) where T : MongoEntity, new();

        void TestConnection(string user, string password, string serverCluster, int port, string databaseName, bool useCosmosDB = false, string costmosDbParam="");
        void DeleteDataBase();
        void DeleteCollection<T>() where T : MongoEntity, new();
        void StartTransaction();
        void CommitTransaction();
        BsonDocument Eq(string propName, object v);
        BsonDocument And(IEnumerable<BsonDocument> ands);
        PaginationModel<T> GetAll<T>(
            FilterDefinition<T>? filter = null, 
            Expression<Func<T, object>>? sort = null, 
            bool sortDesc = false,
            int take = int.MaxValue,
            int skip = 0) where T : MongoEntity, new();
        Task<PaginationModel<T>> GetAllAsync<T>(
            FilterDefinition<T>? filter = null,
            Expression<Func<T, object>>? sort = null,
            bool sortDesc = false,
            int take = int.MaxValue,
            int skip = 0) where T : MongoEntity, new();
        void Update<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : MongoEntity, new();
        void InsertManyDatabases<T>(T doc) where T : MongoEntity, new();
        List<T> GetPropertiesBy<T1,T>(Expression<Func<T1, bool>> where, Expression<Func<T1, T>> prop) where T1 : MongoEntity, new();
    }
}
