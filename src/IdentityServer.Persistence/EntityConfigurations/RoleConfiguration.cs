using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.ToTable("Roles");
		builder.Property(x => x.Id).UseHiLo("roleseq");

		builder.HasMany(x => x.Claims)
			.WithOne(x => x.Role)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(x => x.UserRoles)
			.WithOne(x => x.Role)
			.HasForeignKey(x => x.RoleId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
