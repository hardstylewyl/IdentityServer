using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class UserApplicationConfiguration : IEntityTypeConfiguration<UserApplication>
{
	public void Configure(EntityTypeBuilder<UserApplication> builder)
	{
		builder.ToTable("UserApplications");
		builder.Property(x => x.Id).UseHiLo("userapplicationseq");
	}
}
