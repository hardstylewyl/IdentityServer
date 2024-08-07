using IdentityServer.Domain.Repositories;
using IdentityServer.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Persistence;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString, string migrationsAssembly = "")
	{

		services.AddDbContext<IdentityServerDbContext>(options =>
		{
			options.UseNpgsql(connectionString, sql =>
			{
				if (!string.IsNullOrEmpty(migrationsAssembly))
				{
					sql.MigrationsAssembly(migrationsAssembly);
				}
			});

			options.UseOpenIddict();

		})
			.AddDbContextFactory<IdentityServerDbContext>((Action<DbContextOptionsBuilder>)null, ServiceLifetime.Scoped)
			.AddRepositories();



		return services;
	}


	private static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>))
				.AddScoped(typeof(IAuditLogEntryRepository), typeof(AuditLogEntryRepository))
				.AddScoped(typeof(IUserRepository), typeof(UserRepository))
				.AddScoped(typeof(IRoleRepository), typeof(RoleRepository));

		services.AddScoped(typeof(IUnitOfWork), services =>
		{
			return services.GetRequiredService<IdentityServerDbContext>();
		});

		//services.AddScoped<ILockManager, LockManager>();
		//services.AddScoped<ICircuitBreakerManager, CircuitBreakerManager>();

		return services;
	}


	public static void MigrateIdentityServerDb(this IApplicationBuilder app)
	{
		using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
		serviceScope.ServiceProvider.GetRequiredService<IdentityServerDbContext>().Database.Migrate();
	}

}
