using ApiService.Domain.Databases;

namespace ApiService.Infra.Databases
{
    public class ConnectionContext : IConnectionContext
    {
        private readonly IMongoDBConnection _mongoConnection;
        private readonly PostgreConnection _postgreConnection;
        public ConnectionContext(
            IMongoDBConnection mongoConnection, 
            PostgreConnection postgreConnection)
        {
            _mongoConnection = mongoConnection;
            _postgreConnection = postgreConnection;
        }
        public IMongoDBConnection MongoDBConnection => _mongoConnection;
        public PostgreConnection PostgreConnection => _postgreConnection;
    }
}
