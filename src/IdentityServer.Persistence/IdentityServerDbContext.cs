using System.Data;
using System.Reflection;
using IdentityServer.Domain.Entites.Abstractions;
using IdentityServer.Domain.Repositories;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdentityServer.Persistence;

public class IdentityServerDbContext : DbContext, IUnitOfWork, IDataProtectionKeyContext
{
#nullable disable
	private IDbContextTransaction _dbContextTransaction;

	public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options)
		: base(options)
	{
	}

	public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

	public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
	{
		_dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
		return _dbContextTransaction;
	}

	public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, string lockName = null, CancellationToken cancellationToken = default)
	{
		_dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
		return _dbContextTransaction;
	}

	public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
	{
		await _dbContextTransaction.CommitAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(typeof(IdentityServerDbContext).Assembly);
	}
}
