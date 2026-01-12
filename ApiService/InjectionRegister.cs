using ApiService.Definitions;
using ApiService.Domain.AdminEntities.Tenants;
using ApiService.Domain.AdminEntities.Users;
using ApiService.Domain.Caches;
using ApiService.Domain.Databases;
using ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups;
using ApiService.Domain.Entities.Generics;
using ApiService.Domain.Entities.Users;
using ApiService.Domain.Security;
using ApiService.Domain.Views;
using ApiService.Infra.Databases;
using ApiService.Infra.Entities.Authorizations.AuthorizationGroups;
using ApiService.Infra.Entities.Generics;
using ApiService.Infra.Security;
using ApiService.Infra.Views;
using ApiService.Services.AdminEntities.Tenants;
using ApiService.Services.AdminEntities.Users;
using ApiService.Services.Caches;
using ApiService.Services.Entities.Authorizations.AuthorizationGroups;
using ApiService.Services.Entities.Users;
using ApiService.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ApiService
{
    public class InjectionRegister
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<TokenService>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            services.AddScoped<IConnectionContext, ConnectionContext>();
            services.AddScoped<IMongoDBConnection, MongoDBConnection>();
            services.AddDbContext<PostgreConnection>(options =>
            {
                var connection = Environment.GetEnvironmentVariable(DefaultValues.POSTEGRES_CONNECTION, DefaultValues.EnvironmentTarget);
                options.UseNpgsql(connection);
            });
            services.AddScoped<IClaimContext, ClaimContext>();
            services.AddScoped<IPostgreTransactionManager, PostgreTransactionManager>();

            services.AddTransient<IAuthorizationGroupCache, AuthorizationGroupCache>();

            RegisterRepository(services, configuration);
            RegisterServices(services, configuration);
        }
        public static void RegisterRepository(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IGenericMongoRepository<>), typeof(GenericMongoRepository<>));
            services.AddTransient(typeof(IGenericPostgreRepository<>), typeof(GenericPostgreRepository<>));

            services.AddTransient<IAuthorizationGroupRepository, AuthorizationGroupRepository>();
        }
        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<ICacheService, CacheService>();

            services.AddTransient<IUserAdminService, UserAdminService>();
            services.AddTransient<ITenantService, TenantService>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthorizationGroupService, AuthorizationGroupService>();
        }
    }
}
