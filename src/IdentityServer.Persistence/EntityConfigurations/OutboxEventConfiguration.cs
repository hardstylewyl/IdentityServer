using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
	public void Configure(EntityTypeBuilder<OutboxEvent> builder)
	{
		builder.ToTable("OutboxEvents");
		builder.Property(x => x.Id).UseHiLo("outboxevnetseq");
	}
}
