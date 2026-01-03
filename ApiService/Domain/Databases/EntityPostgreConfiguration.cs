using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Entities;

namespace ApiService.Domain.Databases
{
    public abstract class EntityPostgreConfiguration<T> : IEntityTypeConfiguration<T> where T : PostegresEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnType("uuid")
                   .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.CreateDate).HasColumnType("timestamp with time zone").HasDefaultValueSql("now()").IsRequired();
            builder.Property(x => x.UpdateDate).HasColumnType("timestamp with time zone").HasDefaultValueSql("now()");
        }
    }
}
