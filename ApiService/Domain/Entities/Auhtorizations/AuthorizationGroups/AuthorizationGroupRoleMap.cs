using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Databases;

namespace ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups
{
    public class AuthorizationGroupRoleMap : EntityPostgreConfiguration<AuthorizationGroupRole>
    {
        public void Configure(EntityTypeBuilder<AuthorizationGroupRole> builder)
        {
            builder.ToTable("authorization_group_roles");

            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Role)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.AuthorizationGroupId).HasColumnType("uuid")
                .IsRequired();
        }
    }
}
