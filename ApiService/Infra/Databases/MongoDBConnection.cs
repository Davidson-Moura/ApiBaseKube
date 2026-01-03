using ApiService.Domain.Databases;
using ApiService.Domain.Entities;
using ApiService.Models.Lists;
using System.Linq.Expressions;
using ApiService.Definitions;
using MongoDB.Driver;
using MongoDB.Bson;
using Common;

namespace ApiService.Infra.Databases
{
    public class MongoDBConnection : IMongoDBConnection
    {
        private IMongoClient? _client;
        private IMongoDatabase? _mongoDb;
        public IMongoDatabase MongoDatabase
        {
            get
            {
                if (_mongoDb == null) Connect();
                return _mongoDb;
            }
        }
        public void Connect()
        {
            var user = Environment.GetEnvironmentVariable(DefaultValues.MGUser, DefaultValues.EnvironmentTarget);
            var pwdEncrypted = Environment.GetEnvironmentVariable(DefaultValues.MGPWD, DefaultValues.EnvironmentTarget);

            var url = Environment.GetEnvironmentVariable(DefaultValues.MGUrl, DefaultValues.EnvironmentTarget);
            var portStr = Environment.GetEnvironmentVariable(DefaultValues.MGPort, DefaultValues.EnvironmentTarget);
            var port = 0;
            if (int.TryParse(portStr, out var p)) port = p;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwdEncrypted) || string.IsNullOrEmpty(url))
                throw new SException(Common.Messages.Messages.InvalidConnectionConfiguration);

            string dbName = string.Empty;

            var config =  new MongoConfiguration()
            {
                DBUrl = url,
                DBPort = port,
                DBName = dbName,
                DBUser = user,
                DBPassword = Cryptography.DecryptString(pwdEncrypted)
            };

            config.Validate();

            var connstring = GetMongoSetting(config.DBUser, 
                config.DBUrl, 
                config.DBName, 
                config.DBPort, 
                config.DBPassword);

            try
            {
                _client = new MongoClient(connstring);
                _mongoDb = _client.GetDatabase(config.DBName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro Mongo: {ex.Message}");
            }
        }
        IMongoCollection<T> IMongoDBConnection.GetCollection<T>()
        {
            return GetColl<T>();
        }
        private IMongoCollection<T> GetColl<T>()
        {
            return this.MongoDatabase.GetCollection<T>(typeof(T).Name);
        }
        public void Delete<T>(Guid id) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            if (CurrentSession is not null) collection.DeleteOne(CurrentSession, filter);
            else collection.DeleteOne(filter);
        }
        
        public void Delete<T>(IEnumerable<Guid> ids) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var filter = Builders<T>.Filter.In(s => s.Id, ids);
            if (CurrentSession is not null) collection.DeleteMany(CurrentSession, filter);
            else collection.DeleteMany(filter);
        }
        public async Task DeleteAsync<T>(IEnumerable<Guid> ids) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var filter = Builders<T>.Filter.In(s => s.Id, ids);
            if (CurrentSession is not null) await collection.DeleteManyAsync(CurrentSession, filter);
            else await collection.DeleteManyAsync(filter);
        }
        public async Task DeleteAsync<T>(Guid id) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var filter = Builders<T>.Filter.Eq(s => s.Id, id);
            if (CurrentSession is not null) await collection.DeleteOneAsync(CurrentSession, filter);
            else await collection.DeleteOneAsync(filter);
        }
        public List<T> GetAll<T>() where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var documents =
                CurrentSession is not null ?
                collection.Find(CurrentSession, _ => true).Limit(DefaultValues.MaxEntityPerRequest).ToListAsync()
                : collection.Find(_ => true).Limit(DefaultValues.MaxEntityPerRequest).ToListAsync();
            return documents.Result;
        }
        public T? GetById<T>(Guid id) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var documents =
                CurrentSession is not null ?
                collection.Find(CurrentSession, e => e.Id == id)
                : collection.Find(e => e.Id == id);

            return documents.ToListAsync().Result.FirstOrDefault();
        }
        public async Task<T> GetByIdAsync<T>(Guid id) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var documents =
                CurrentSession is not null ?
                collection.Find(CurrentSession, e => e.Id == id)
                : collection.Find(e => e.Id == id);

            return await documents.FirstOrDefaultAsync();
        }
        public long GetCount<T>(Expression<Func<T, bool>> expression) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var count =
                CurrentSession is not null ?
                collection.CountDocumentsAsync(CurrentSession, expression).Result
                : collection.CountDocumentsAsync(expression).Result;
            return count;
        }
        public long GetCount<T>(FilterDefinition<T> filter) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var count =
                CurrentSession is not null ?
                collection.CountDocumentsAsync(CurrentSession, filter).Result
                : collection.CountDocumentsAsync(filter).Result;
            return count;
        }
        public long GetEstimedCount<T>() where T : MongoEntity, new()
        {
            var count = GetColl<T>().EstimatedDocumentCount();
            return count;
        }
        public void Insert<T>(T record) where T : MongoEntity, new()
        {
            record.GenerateId();
            record.CreateDate = DateTime.Now;
            var collection = GetColl<T>();
            if (CurrentSession is not null) collection.InsertOne(CurrentSession, record);
            else collection.InsertOne(record);
        }
        public void InsertMany<T>(IEnumerable<T> records) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            if (CurrentSession is not null) collection.InsertMany(CurrentSession, records);
            else collection.InsertMany(records);
        }
        public async Task InsertManyAsync<T>(IEnumerable<T> records) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            if (CurrentSession is not null) await collection.InsertManyAsync(CurrentSession, records);
            else await collection.InsertManyAsync(records);
        }
        public async Task InsertAsync<T>(T record) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            if (CurrentSession is not null)
                await collection.InsertOneAsync(CurrentSession, record);
            else
                await collection.InsertOneAsync(record);
        }
        public void TestConnection(string user, string password, string serverCluster, int port, string databaseName, bool useCosmosDB = false, string cosmosDbParam = "")
        {
            var mongoPassword = Uri.EscapeDataString(password);

            var setting = GetMongoSetting(user, serverCluster, databaseName, port, mongoPassword);

            var client = new MongoClient(setting);
            client.GetDatabase(databaseName);
            client.ListDatabaseNames();
        }
        private static MongoClientSettings GetMongoSetting(string user, string serverCluster,
            string databaseName,
            int port, string mongoPassword)
        {
            var settings = MongoClientSettings.FromConnectionString( GetConnectionString(user, serverCluster, port, mongoPassword, databaseName) );

            if(!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(mongoPassword) && port > 0)
            {
                MongoCredential credential = MongoCredential.CreateCredential(databaseName, user, mongoPassword);
                settings.Credential = credential;
                settings.ReplicaSetName = "rs0";
                settings.DirectConnection = true;
                settings.SslSettings.EnabledSslProtocols = System.Security.Authentication.SslProtocols.None;
            }

            return settings;
        }
        private static string GetConnectionString(string user, string serverCluster,
            int port, string mongoPassword, string? databaseName = null)
        {
            string connstring = string.Empty;

            if(port > 0 && string.IsNullOrEmpty(user) && string.IsNullOrEmpty(mongoPassword))
                connstring = $"mongodb://{serverCluster}:{port}/?directConnection=true";
            else if (port > 0)
                connstring = $"mongodb://{user}:{mongoPassword}@{serverCluster}:{port}/";
            else
                connstring = $"mongodb+srv://{user}:{mongoPassword}@{serverCluster}/?retryWrites=true&w=majority";
            
            return connstring;
        }
        public void Update<T>(T record) where T : MongoEntity, new()
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, record.Id);
            record.UpdateDate = DateTime.Now;

            var document =
                CurrentSession is not null ? MongoDatabase.GetCollection<T>(typeof(T).Name).ReplaceOne(CurrentSession, filter, record)
                : MongoDatabase.GetCollection<T>(typeof(T).Name).ReplaceOne(filter, record);
        }
        public async Task UpdateAsync<T>(T record) where T : MongoEntity, new()
        {
            var filter = Builders<T>.Filter.Eq(s => s.Id, record.Id);
            record.UpdateDate = DateTime.Now;

            var document = await
                (
                    CurrentSession is not null ? MongoDatabase.GetCollection<T>(typeof(T).Name).ReplaceOneAsync(CurrentSession, filter, record)
                    : MongoDatabase.GetCollection<T>(typeof(T).Name).ReplaceOneAsync(filter, record)
                );
        }
        public void Update<T>(FilterDefinition<T> filter, UpdateDefinition<T> update) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            update.Set(x => x.UpdateDate, DateTime.Now);

            if (CurrentSession is not null) collection.UpdateOne(CurrentSession, filter, update);
            else collection.UpdateOne(filter, update);
        }
        IEnumerable<T> IMongoDBConnection.GetByFilter<T>(Expression<Func<T, bool>> expression)
        {
            var collection = GetColl<T>();
            var documents =
                CurrentSession is not null ?
                collection.Find(CurrentSession, expression)
                : collection.Find(expression);
            return documents.ToListAsync().Result;
        }
        IEnumerable<T> IMongoDBConnection.GetByFilter<T>(FilterDefinition<T> filter)
        {
            var collection = GetColl<T>();

            var documents = CurrentSession is not null ?
                collection.Find(CurrentSession, filter)
                : collection.Find(filter);
            return documents.ToListAsync().Result;
        }
        IEnumerable<T> IMongoDBConnection.GetByFilterOrderByAsc<T>(string propertyName, Expression<Func<T, bool>> expression)
        {
            var collection = GetColl<T>();
            var sort = Builders<T>.Sort.Ascending(propertyName);
            var documents =
                CurrentSession is not null ?
                collection.Find(CurrentSession, expression).Sort(sort)
                : collection.Find(expression).Sort(sort);
            return documents.ToListAsync().Result;
        }
        IEnumerable<T> IMongoDBConnection.GetByFilterOrderByDesc<T>(string propertyName, Expression<Func<T, bool>> expression)
        {
            var collection = GetColl<T>();
            var sort = Builders<T>.Sort.Descending(propertyName);
            var documents =
                CurrentSession is not null ?
                collection.Find(CurrentSession, expression).Sort(sort)
                : collection.Find(expression).Sort(sort);
            return documents.ToListAsync().Result;
        }
        IEnumerable<T> IMongoDBConnection.GetByFilterOrderByDesc<T>(string propertyName, FilterDefinition<T> filter)
        {
            var collection = GetColl<T>();
            var sort = Builders<T>.Sort.Descending(propertyName);
            var documents =
                CurrentSession is not null ? collection.Find(CurrentSession, filter).Sort(sort)
                : collection.Find(filter).Sort(sort);
            return documents.ToListAsync().Result;
        }
        public bool Exists<T>(Expression<Func<T, bool>> expression) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            return CurrentSession is not null ?
                collection.Find(CurrentSession, expression).Any()
                : collection.Find(expression).Any();
        }
        public bool Any<T>() where T : MongoEntity, new()
        {
            return GetColl<T>().Find(_ => true).Any();
        }
        public T GetLast<T>() where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var sort = Builders<T>.Sort.Descending(c => c.Id);
            return
                CurrentSession is not null ? 
                collection.Find(CurrentSession, _ => true).Sort(sort).Limit(1).FirstOrDefault()
                : collection.Find(_ => true).Sort(sort).Limit(1).FirstOrDefault();
        }
        public T2 GetProperty<T, T2>(Guid id, Expression<Func<T, T2>> exp) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var v =
                CurrentSession is not null ?
                collection.Find(CurrentSession, e => e.Id == id).Project(exp)
                : collection.Find(e => e.Id == id).Project(exp);

            return v.FirstOrDefault();
        }
        public T2 GetPropertyBy<T, T2>(Expression<Func<T, bool>> where, Expression<Func<T, T2>> prop) where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            var v =
            CurrentSession is not null ?
                collection.Find(CurrentSession, where).Project(prop).FirstOrDefault()
                : collection.Find(where).Project(prop).FirstOrDefault();

            return v;
        }
        public List<T> GetPropertiesBy<T1, T>(Expression<Func<T1, bool>> where, Expression<Func<T1, T>> prop) where T1 : MongoEntity, new()
        {
            var collection = GetColl<T1>();
            var v =
            CurrentSession is not null ?
                collection.Find(CurrentSession, where).Project(prop).ToList()
                : collection.Find(where).Project(prop).ToList();

            return v;
        }
        /*
        private IMongoCollection<DocumentMongoLog<T>> GetCollectionLogName<T>() where T : MongoEntity, new() 
            => this.MongoDatabase.GetCollection<DocumentMongoLog<T>>($"{typeof(T).Name}_Log") ;
        public IEnumerable<DocumentMongoLog<T>> GetEntityLogByFilter<T>(FilterDefinition<DocumentMongoLog<T>> filter) where T : IMongoDocument, new()
        {
            var collection = GetCollectionLogName<T>();
            var entityName = typeof(T).Name;

            var result = CurrentSession is not null ? 
                collection.Find(CurrentSession, filter).SortByDescending(x => x.DateCreate).ToList() 
                : collection.Find(filter).SortByDescending(x => x.DateCreate).ToList();

            return result;
        }
        public long CountEntityLogByFilter<T>(FilterDefinition<DocumentMongoLog<T>> filter) where T : class, IMongoDocument, new()
        {
            var collection = GetCollectionLogName<T>();
            var count =
                CurrentSession is not null ?
                collection.CountDocumentsAsync(CurrentSession, filter).Result
                : collection.CountDocumentsAsync(filter).Result;
            return count;
        }
        public async void InsertLogAsync<T>(DocumentMongoLog<T> record) where T : class, IMongoDocument, new()
        {
            var collection = GetCollectionLogName<T>();

            if (CurrentSession is not null) await collection.InsertOneAsync(CurrentSession, record);
            else await collection.InsertOneAsync(record);
        }
        */
        public async void DeleteDataBase()
        {
            var collectionNames = MongoDatabase.ListCollectionNames();

            foreach (var collectionName in collectionNames.ToList())
            {
                var collection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                _ = collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty)
                    .ContinueWith(t =>
                    {
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }
        public async void DeleteCollection<T>() where T : MongoEntity, new()
        {
            var collection = GetColl<T>();

            if (CurrentSession is not null) collection.DeleteMany(CurrentSession, x => true);
            else collection.DeleteMany(x => true);
        }
        private IClientSessionHandle? _currentSession;
        public IClientSessionHandle CurrentSession
        {
            get
            {
                if (_currentSession is not null && _currentSession.ServerSession is null) _currentSession = null;
                return _currentSession;
            }
            set
            {
                _currentSession = value;
            }
        }
        private IClientSessionHandle StartSession()
        {
            if (CurrentSession is null) CurrentSession = MongoDatabase.Client.StartSession();
            return CurrentSession;
        }
        private int _countTransaction = 0;
        public void StartTransaction()
        {
            var ssn = StartSession();
            if (!ssn.IsInTransaction) ssn.StartTransaction();
            _countTransaction++;
        }
        public void CommitTransaction()
        {
            _countTransaction--;
            if (_countTransaction <= 0)
            {
                var ssn = StartSession();
                if (ssn.IsInTransaction) ssn.CommitTransaction();
            }
        }
        public void Dispose()
        {
            if (CurrentSession?.IsInTransaction ?? false && _countTransaction > 0) CurrentSession.AbortTransaction();
        }
        public BsonDocument Eq(string propName, object v)
        {
            return new BsonDocument("$expr",
                new BsonDocument("$eq",
                    new BsonArray { $"${propName}", BsonValue.Create(v) })
                );
        }
        public BsonDocument And(IEnumerable<BsonDocument> ands)
        {
            return new BsonDocument("$match",
                new BsonDocument("$expr",
                    new BsonDocument("$and", new BsonArray(ands))
                    )
                );
        }

        public long Count<T>(FilterDefinition<T>? filter = null) where T : MongoEntity, new()
        {
            if (filter is null) filter = Builders<T>.Filter.Empty;
            var collection = GetColl<T>();
            return _currentSession is not null ? collection.CountDocuments(_currentSession, filter) : collection.CountDocuments(filter);
        }
        public async Task<long> CountAsync<T>(FilterDefinition<T>? filter = null) where T : MongoEntity, new()
        {
            if (filter is null) filter = Builders<T>.Filter.Empty;
            var collection = GetColl<T>();
            return await (_currentSession is not null ? collection.CountDocumentsAsync(_currentSession, filter) : collection.CountDocumentsAsync(filter));
        }
        public long EstimedCount<T>() where T : MongoEntity, new()
        {
            var collection = GetColl<T>();
            return collection.EstimatedDocumentCount();
        }
        public IEnumerable<T> Get<T>(
            FilterDefinition<T>? filter = null, 
            int take = int.MaxValue, 
            int skip = 0,
            Expression<Func<T, object>>? sort = null,
            bool sortDesc = false
            ) where T : MongoEntity, new()
        {
            if (filter is null) filter = Builders<T>.Filter.Empty;
            var collection = GetColl<T>();
            var find = _currentSession is not null ? collection.Find(_currentSession, filter) : collection.Find(filter);

            if (sort is not null) find = sortDesc ? find.SortByDescending(sort) : find.SortBy(sort);

            return find.Limit(take).Skip(skip).ToEnumerable();
        }
        public async Task<IEnumerable<T>> GetAsync<T>(
            FilterDefinition<T>? filter = null,
            int take = int.MaxValue,
            int skip = 0,
            Expression<Func<T, object>>? sort = null,
            bool sortDesc = false
            ) where T : MongoEntity, new()
        {
            if (filter is null) filter = Builders<T>.Filter.Empty;
            var collection = GetColl<T>();
            var find = _currentSession is not null ? collection.Find(_currentSession, filter) : collection.Find(filter);

            if (sort is not null) find = sortDesc ? find.SortByDescending(sort) : find.SortBy(sort);

            return await find.Limit(take).Skip(skip).ToListAsync();
        }
        public PaginationModel<T> GetAll<T>(
            FilterDefinition<T>? filter = null, 
            Expression<Func<T, object>>? sort = null,
            bool sortDesc = false,
            int take = int.MaxValue,
            int skip = 0
            ) where T : MongoEntity, new()
        {
            if (filter is null) filter = Builders<T>.Filter.Empty;

            var model = new PaginationModel<T>()
            {
                List = Get<T>(filter, take: take, skip: skip, sort: sort, sortDesc: sortDesc),
                Count = (int)Count<T>(filter)
            };

            return model;
        }
        public async Task<PaginationModel<T>> GetAllAsync<T>(
            FilterDefinition<T>? filter = null,
            Expression<Func<T, object>>? sort = null,
            bool sortDesc = false,
            int take = int.MaxValue,
            int skip = 0
            ) where T : MongoEntity, new()
        {
            if (filter is null) filter = Builders<T>.Filter.Empty;

            var model = new PaginationModel<T>()
            {
                List = await GetAsync<T>(filter, take: take, skip: skip, sort: sort, sortDesc: sortDesc),
                Count = (int) (await CountAsync<T>(filter))
            };

            return model;
        }

        public void InsertManyDatabases<T>(T doc) where T : MongoEntity, new()
        {
            throw new NotImplementedException();
            var dtbs = this.MongoDatabase;
            var databaseNames = _client.ListDatabaseNames().ToList();

            foreach (var dbName in databaseNames)
            {
                try
                {
                    var db = _client.GetDatabase(dbName);

                    var collections = db.ListCollectionNames().ToList();
                    var collectionName = typeof(T).Name;
                    if (collections.Contains(collectionName))
                    {
                        var collection = db.GetCollection<T>(collectionName);

                        collection.InsertOne(doc);
                    }
                }
                catch (Exception ex) { }
            }
        }
    }
}
