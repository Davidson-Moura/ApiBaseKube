using ApiService.Domain.Entities.Generics;
using ApiService.Infra.Entities.Generics;
using ApiService.Services.Entities.Users;
using ApiService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using ApiService.Services.Security;
using ApiService.Domain.Databases;
using ApiService.Domain.Security;
using ApiService.Infra.Databases;
using ApiService.Infra.Security;
using ApiService.Definitions;

namespace ApiService
{
    public class InjectionRegister
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<TokenService>();

            services.AddScoped<IConnectionContext, ConnectionContext>();
            services.AddScoped<IMongoDBConnection, MongoDBConnection>();
            services.AddDbContext<PostgreConnection>(options =>
            {
                var connection = Environment.GetEnvironmentVariable(DefaultValues.POSTEGRES_CONNECTION, DefaultValues.EnvironmentTarget);
                options.UseNpgsql(connection);
            });
            services.AddScoped<IClaimContext, ClaimContext>();
            services.AddScoped<IPostgreTransactionManager, PostgreTransactionManager>();

            RegisterRepository(services, configuration);
            RegisterServices(services, configuration);
        }
        public static void RegisterRepository(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IGenericMongoRepository<>), typeof(GenericMongoRepository<>));
            services.AddTransient(typeof(IGenericPostgreRepository<>), typeof(GenericPostgreRepository<>));
        }
        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IUserService, UserService>();
        }
    }
}
