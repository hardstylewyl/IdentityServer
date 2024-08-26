using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class UserLinkConfiguration : IEntityTypeConfiguration<UserLink>
{
	public void Configure(EntityTypeBuilder<UserLink> builder)
	{
		builder.ToTable("UserLinks");
		builder.Property(x => x.Id).UseHiLo("userlinkseq");
	}
}
