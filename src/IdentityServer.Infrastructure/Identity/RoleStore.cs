using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Infrastructure.Identity;

public class RoleStore : IRoleStore<Role>
{

	private readonly IUnitOfWork _unitOfWork;
	private readonly IRoleRepository _roleRepository;

	public RoleStore(IUnitOfWork unitOfWork, IRoleRepository roleRepository)
	{
		_unitOfWork = unitOfWork;
		_roleRepository = roleRepository;
	}

	public void Dispose()
	{
	}

	public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
	{
		await _roleRepository.AddOrUpdateAsync(role, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return IdentityResult.Success;
	}

	public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
	{
		_roleRepository.Delete(role);
		return Task.FromResult(IdentityResult.Success);
	}

	public async Task<Role?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
	{
		if (!long.TryParse(roleId, out var id))
		{
			return null;
		}

		return await _roleRepository.Get(new RoleQueryOptions { })
			.FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<Role?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
	{
		return await _roleRepository.Get(new RoleQueryOptions { })
			.FirstOrDefaultAsync(x => x.NormalizedName == normalizedRoleName);
	}

	public Task<string?> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
	{
		return Task.FromResult(role.NormalizedName);
	}

	public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
	{
		return Task.FromResult(role.Id.ToString());
	}

	public Task<string?> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
	{
		return Task.FromResult(role.Name);
	}

	public Task SetNormalizedRoleNameAsync(Role role, string? normalizedName, CancellationToken cancellationToken)
	{
		role.NormalizedName = normalizedName;
		return Task.CompletedTask;
	}

	public Task SetRoleNameAsync(Role role, string? roleName, CancellationToken cancellationToken)
	{
		role.Name = roleName;
		return Task.CompletedTask;
	}

	public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
	{
		await _roleRepository.UpdateAsync(role, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return IdentityResult.Success;
	}
}
