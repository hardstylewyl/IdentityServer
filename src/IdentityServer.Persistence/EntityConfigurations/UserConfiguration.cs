using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");
		builder.Property(x => x.Id).UseHiLo("userseq");

		builder.HasMany(x => x.Claims)
			.WithOne(x => x.User)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(x => x.UserRoles)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		//builder.HasMany(x => x.UserLinks)
		//.WithOne()
		//.HasForeignKey(x => x.UserId)
		//.OnDelete(DeleteBehavior.Cascade);

		//builder.HasMany(x => x.UserLoginHistories)
		//.WithOne()
		//.HasForeignKey(x => x.UserId)
		//.OnDelete(DeleteBehavior.Cascade);

		//builder.HasMany(x => x.Tokens)
		//.WithOne()
		//.HasForeignKey(x => x.UserId)
		//.OnDelete(DeleteBehavior.Cascade);
	}
}
