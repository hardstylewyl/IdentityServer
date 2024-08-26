using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
	public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
	{
		builder.ToTable("AuditLogEntries");
		builder.Property(x => x.Id).UseHiLo("auditlogentryseq");
	}
}
