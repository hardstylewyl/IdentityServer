using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class LocalizationEntryConfiguration : IEntityTypeConfiguration<LocalizationEntry>
{
	public void Configure(EntityTypeBuilder<LocalizationEntry> builder)
	{
		builder.ToTable("LocalizationEntries");
		builder.Property(x => x.Id).UseHiLo("localizationseq");
	}
}
