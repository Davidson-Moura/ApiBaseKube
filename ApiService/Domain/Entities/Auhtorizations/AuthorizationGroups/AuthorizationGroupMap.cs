using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ApiService.Domain.Databases;

namespace ApiService.Domain.Entities.Auhtorizations.AuthorizationGroups
{
    public class AuthorizationGroupMap : EntityPostgreConfiguration<AuthorizationGroup>
    {
        public void Configure(EntityTypeBuilder<AuthorizationGroup> builder)
        {
            builder.ToTable("authorization_groups");

            base.Configure(builder);

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();

            builder.Property(x => x.IsSystem).HasColumnType("boolean").IsRequired();

            builder.HasMany(x => x.Roles)
                .WithOne(x => x.AuthorizationGroup)
                .HasForeignKey(x => x.AuthorizationGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
