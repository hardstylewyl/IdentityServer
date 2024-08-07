using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
	public void Configure(EntityTypeBuilder<RoleClaim> builder)
	{
		builder.ToTable("RoleClaims");
		builder.Property(x => x.Id).UseHiLo("roleclaimseq");
	}
}
