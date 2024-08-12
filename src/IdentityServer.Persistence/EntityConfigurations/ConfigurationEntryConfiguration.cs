using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class ConfigurationEntryConfiguration : IEntityTypeConfiguration<ConfigurationEntry>
{
	public void Configure(EntityTypeBuilder<ConfigurationEntry> builder)
	{
		builder.ToTable("ConfigurationEntries");
		builder.Property(x => x.Id).UseHiLo("configurationentryseq");
	}
}
