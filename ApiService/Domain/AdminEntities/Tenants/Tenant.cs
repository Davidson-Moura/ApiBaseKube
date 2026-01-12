using ApiService.Domain.Entities;

namespace ApiService.Domain.AdminEntities.Tenants
{
    public class Tenant : PostgresEntity
    {
        public string? Name { get; set; }
    }
}
