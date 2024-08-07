using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
	public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
	{
		builder.ToTable("AuditLogEntries");
		builder.Property(x => x.Id).UseHiLo("auditlogentryseq");
	}
}
