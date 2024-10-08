using System.Security.Claims;
using IdentityServer.CrossCuttingConcerns.Utility;
using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Infrastructure.Identity;

public class UserStore :
	IUserRoleStore<User>,
	IUserLoginStore<User>,
	IUserClaimStore<User>,
	IUserPasswordStore<User>,
	IUserSecurityStampStore<User>,
	IUserEmailStore<User>,
	IUserLockoutStore<User>,
	IUserPhoneNumberStore<User>,
	IQueryableUserStore<User>,
	IUserTwoFactorStore<User>,
	IUserAuthenticationTokenStore<User>,
	IUserAuthenticatorKeyStore<User>,
	IUserTwoFactorRecoveryCodeStore<User>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;
	private readonly IRoleRepository _roleRepository;

	public UserStore(IUserRepository userRepository, IRoleRepository roleRepository)
	{
		_unitOfWork = userRepository.UnitOfWork;
		_userRepository = userRepository;
		_roleRepository = roleRepository;
	}

	public IQueryable<User> Users => _userRepository.GetQueryableSet();
	public IUserRepository UserRepository => _userRepository;

	public void Dispose()
	{
	}

	public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
	{
		user.PasswordHistories = new List<PasswordHistory>()
		{
			new PasswordHistory
			{
				PasswordHash = user.PasswordHash,
				CreatedDateTime = DateTimeOffset.Now,
			},
		};
		await _userRepository.AddOrUpdateAsync(user, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return IdentityResult.Success;
	}

	public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
	{
		_userRepository.Delete(user);
		return Task.FromResult(IdentityResult.Success);
	}

	public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
	{
		return _userRepository.Get(new UserQueryOptions { IncludeTokens = true })
			.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
	}

	public async Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken)
	{
		if (!long.TryParse(userId, out var id))
		{
			return null;
		}

		return await _userRepository.Get(new UserQueryOptions { IncludeTokens = true })
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
	{
		return _userRepository.Get(new UserQueryOptions { IncludeTokens = true })
			.FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
	}

	public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.AccessFailedCount);
	}

	public Task<string?> GetEmailAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.Email);
	}

	public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.EmailConfirmed);
	}

	public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.LockoutEnabled);
	}

	public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.LockoutEnd);
	}

	public Task<string?> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.NormalizedEmail);
	}

	public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.NormalizedUserName);
	}

	public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.PasswordHash);
	}

	public Task<string?> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.PhoneNumber);
	}

	public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.PhoneNumberConfirmed);
	}

	public Task<string?> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.SecurityStamp);
	}

	public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.TwoFactorEnabled);
	}

	public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.Id.ToString());
	}

	public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.UserName);
	}

	public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.PasswordHash != null);
	}

	public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
	{
		user.AccessFailedCount++;
		return Task.FromResult(user.AccessFailedCount);
	}

	public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
	{
		user.AccessFailedCount = 0;
		return Task.CompletedTask;
	}

	public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
	{
		user.Email = email;
		return Task.CompletedTask;
	}

	public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
	{
		user.EmailConfirmed = confirmed;
		return Task.CompletedTask;
	}

	public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
	{
		user.LockoutEnabled = enabled;
		return Task.CompletedTask;
	}

	public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
	{
		user.LockoutEnd = lockoutEnd;
		return Task.CompletedTask;
	}

	public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
	{
		user.NormalizedEmail = normalizedEmail;
		return Task.CompletedTask;
	}

	public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
	{
		user.NormalizedUserName = normalizedName;
		return Task.CompletedTask;
	}

	public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
	{
		user.PasswordHash = passwordHash;
		return Task.CompletedTask;
	}

	public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
	{
		user.PhoneNumber = phoneNumber;
		return Task.CompletedTask;
	}

	public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
	{
		user.PhoneNumberConfirmed = confirmed;
		return Task.CompletedTask;
	}

	public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
	{
		user.SecurityStamp = stamp;
		return Task.CompletedTask;
	}

	public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
	{
		user.TwoFactorEnabled = enabled;
		return Task.CompletedTask;
	}

	public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
	{
		user.UserName = userName;
		return Task.CompletedTask;
	}

	public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
	{
		await _userRepository.AddOrUpdateAsync(user, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return IdentityResult.Success;
	}

	private const string AuthenticatorStoreLoginProvider = "[AuthenticatorStore]";
	private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
	private const string RecoveryCodeTokenName = "RecoveryCodes";

	public Task<string?> GetTokenAsync(User user, string loginProvider, string name,
		CancellationToken cancellationToken)
	{
		var tokenEntity = user.Tokens.SingleOrDefault(
			l => l.TokenName == name && l.LoginProvider == loginProvider);
		return Task.FromResult(tokenEntity?.TokenValue);
	}

	public async Task SetTokenAsync(User user, string loginProvider, string name, string? value,
		CancellationToken cancellationToken)
	{
		var tokenEntity = user.Tokens.SingleOrDefault(
			l => l.TokenName == name && l.LoginProvider == loginProvider);
		if (tokenEntity != null)
		{
			tokenEntity.TokenValue = value;
		}
		else
		{
			user.Tokens.Add(new UserToken
			{
				UserId = user.Id,
				LoginProvider = loginProvider,
				TokenName = name,
				TokenValue = value,
			});
		}

		await _unitOfWork.SaveChangesAsync(cancellationToken);
	}

	public async Task RemoveTokenAsync(User user, string loginProvider, string name,
		CancellationToken cancellationToken)
	{
		var tokenEntity = user.Tokens.SingleOrDefault(
			l => l.TokenName == name && l.LoginProvider == loginProvider);
		if (tokenEntity != null)
		{
			user.Tokens.Remove(tokenEntity);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}
	}

	public Task SetAuthenticatorKeyAsync(User user, string key, CancellationToken cancellationToken)
	{
		return SetTokenAsync(user, AuthenticatorStoreLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
	}

	public Task<string?> GetAuthenticatorKeyAsync(User user, CancellationToken cancellationToken)
	{
		return GetTokenAsync(user, AuthenticatorStoreLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
	}

	public Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
	{
		var mergedCodes = string.Join(";", recoveryCodes);
		return SetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, mergedCodes,
			cancellationToken);
	}

	public async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
	{
		var mergedCodes =
			await GetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, cancellationToken) ??
			string.Empty;
		var splitCodes = mergedCodes.Split(';');
		if (splitCodes.Contains(code))
		{
			var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
			await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
			return true;
		}

		return false;
	}

	public async Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
	{
		var mergedCodes =
			await GetTokenAsync(user, AuthenticatorStoreLoginProvider, RecoveryCodeTokenName, cancellationToken) ??
			string.Empty;
		if (mergedCodes.Length > 0)
		{
			return mergedCodes.Split(';').Length;
		}

		return 0;
	}

	public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
	{
		user = await _userRepository.Get(new UserQueryOptions { IncludeClaims = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		IList<Claim> claims = user.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
		return claims;
	}

	public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
	{
		user = await _userRepository.Get(new UserQueryOptions { IncludeClaims = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		foreach (var claim in claims)
		{
			user.Claims.Add(new UserClaim { Type = claim.Type, Value = claim.Value, User = user });
		}
	}

	public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
	{
		user = await _userRepository.Get(new UserQueryOptions { IncludeClaims = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		var matchedClaims = user.Claims.Where(uc => uc.Value == claim.Value && uc.Type == claim.Type);
		foreach (var matchedClaim in matchedClaims)
		{
			matchedClaim.Value = newClaim.Value;
			matchedClaim.Type = newClaim.Type;
		}
	}

	public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
	{
		user = await _userRepository.Get(new UserQueryOptions { IncludeClaims = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		foreach (var claim in claims)
		{
			var matchedClaims = user.Claims.Where(uc => uc.Value == claim.Value && uc.Type == claim.Type);
			foreach (var c in matchedClaims)
			{
				user.Claims.Remove(c);
			}
		}
	}

	public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
	{
		var query = _userRepository.Get(new UserQueryOptions { IncludeClaims = true })
			.Where(u => u.Claims.Any(x => x.Type == claim.Type && x.Value == claim.Value));

		return await query.ToListAsync(cancellationToken);
	}

	public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
	{
		user = await _userRepository
			.Get(new UserQueryOptions { IncludeUserLinks = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		user.UserLinks.Add(new UserLink
		{
			LoginProvider = login.LoginProvider,
			ProviderDisplayName = login.ProviderDisplayName,
			ProviderKey = login.ProviderKey,
			UserId = user.Id,
			LoginName = "None"
		});
	}

	public async Task RemoveLoginAsync(User user, string loginProvider, string providerKey,
		CancellationToken cancellationToken)
	{
		user = await _userRepository
			.Get(new UserQueryOptions { IncludeUserLinks = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		var entry = user.UserLinks
			.FirstOrDefault(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey);
		if (entry != null)
		{
			user.UserLinks.Remove(entry);
		}
	}

	public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
	{
		user = await _userRepository
			.Get(new UserQueryOptions { IncludeUserLinks = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		return user.UserLinks
			.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName))
			.ToList();
	}

	public Task<User?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
	{
		return _userRepository
			.Get(new UserQueryOptions { IncludeUserLinks = true })
			.FirstOrDefaultAsync(x => x.UserLinks
				.Any(y => y.LoginProvider == loginProvider
				          && y.ProviderKey == providerKey), cancellationToken);
	}

	public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
	{
		Ensure.NotEmpty(roleName, "roleName not empty", nameof(roleName));
		
		var role = await _roleRepository.GetQueryableSet()
			.FirstOrDefaultAsync(x => x.NormalizedName == roleName, cancellationToken);
		if (role == null)
		{
			throw new InvalidOperationException("Role not found");
		}

		user = await UserRepository
			.Get(new UserQueryOptions { IncludeUserRoles = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		user.UserRoles.Add(new UserRole
		{
			UserId = user.Id,
			User = user,
			Role = role,
			RoleId = role.Id
		});
	}

	public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
	{
		Ensure.NotEmpty(roleName, "roleName not empty", nameof(roleName));
		var role = await _roleRepository.GetQueryableSet()
			.FirstOrDefaultAsync(x => x.NormalizedName == roleName, cancellationToken);

		if (role == null)
		{
			throw new InvalidOperationException("Role not found");
		}

		user = await UserRepository
			.Get(new UserQueryOptions { IncludeUserRoles = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		var userRole = user.UserRoles.FirstOrDefault(x => x.RoleId == role.Id);
		if (userRole != null)
		{
			user.UserRoles.Remove(userRole);
		}
	}

	public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
	{
		user = await UserRepository
			.Get(new UserQueryOptions { IncludeUserRoles = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		return user.UserRoles.Select(x => x.Role.Name).ToList();
	}

	public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
	{
		Ensure.NotEmpty(roleName, "roleName not empty", nameof(roleName));

		user = await UserRepository
			.Get(new UserQueryOptions { IncludeUserRoles = true })
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		return user.UserRoles.Any(x => x.Role.NormalizedName == roleName);
	}

	public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
	{
		Ensure.NotEmpty(roleName, "roleName not empty", nameof(roleName));
		var role = await _roleRepository.GetQueryableSet()
			.FirstOrDefaultAsync(x => x.NormalizedName == roleName, cancellationToken);

		if (role == null)
		{
			throw new InvalidOperationException("Role not found");
		}

		return await UserRepository
			.Get(new UserQueryOptions { IncludeUserRoles = true, AsNoTracking = true })
			.Where(x => x.UserRoles.Any(y => y.RoleId == role.Id))
			.ToListAsync(cancellationToken);
	}
}
