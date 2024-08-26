using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class LocalizationConfiguration : IEntityTypeConfiguration<LocalizationEntry>
{
	public void Configure(EntityTypeBuilder<LocalizationEntry> builder)
	{
		builder.ToTable("LocalizationEntries");
		builder.Property(x => x.Id).UseHiLo("localizationseq");
	}
}
