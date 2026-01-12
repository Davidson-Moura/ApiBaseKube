using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Databases;

namespace ApiService.Domain.Entities.Users
{
    public class UserMap : EntityPostgreConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            base.Configure(builder);

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();

            builder.Property(x => x.Email).HasMaxLength(200).IsRequired();

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Password).HasColumnType("text").IsRequired();

            builder.Property(x => x.AuthorizationGroupId).HasColumnType("uuid");
        }
    }
}
