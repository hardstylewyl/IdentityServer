using IdentityServer.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
{
	public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
	{
		builder.ToTable("UserLoginHistories");
		builder.OwnsOne(x => x.IpInfo);
		builder.Property(x => x.Id).UseHiLo("userloginhistoryseq");
	}
}
