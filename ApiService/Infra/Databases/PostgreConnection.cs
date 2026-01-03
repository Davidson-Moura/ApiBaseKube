using ApiService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Entities;

namespace ApiService.Infra.Databases
{
    public class PostgreConnection : DbContext
    {
        public PostgreConnection(DbContextOptions<PostgreConnection> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public bool ThereAreChanges { get; private set; }
        public void MarkChanges() => ThereAreChanges = true;
        public DbSet<T> GetEntity<T>() where T : PostegresEntity => Set<T>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgreConnection).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
