using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class CustomMigrationHistoryConfiguration : IEntityTypeConfiguration<CustomMigrationHistory>
{
	public void Configure(EntityTypeBuilder<CustomMigrationHistory> builder)
	{
		builder.ToTable("_CustomMigrationHistories");
		builder.Property(x => x.Id).UseHiLo("custommigrationhistoryseq");
	}
}
