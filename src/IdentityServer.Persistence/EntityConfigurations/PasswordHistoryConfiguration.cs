using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
{
	public void Configure(EntityTypeBuilder<PasswordHistory> builder)
	{
		builder.ToTable("PasswordHistories");
		builder.Property(x => x.Id).UseHiLo("passwordhistoryseq");
	}
}
