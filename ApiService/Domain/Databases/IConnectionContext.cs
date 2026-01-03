using ApiService.Infra.Databases;

namespace ApiService.Domain.Databases
{
    public interface IConnectionContext
    {
        IMongoDBConnection MongoDBConnection { get; }
        PostgreConnection PostgreConnection { get; }
    }
}
