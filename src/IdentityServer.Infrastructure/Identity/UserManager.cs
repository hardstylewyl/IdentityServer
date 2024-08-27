using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Identity;
using IdentityServer.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.Infrastructure.Identity;

public sealed class UserManager(
	IUserRepository userRepository,
	IUserStore<User> store,
	IOptions<IdentityOptions> optionsAccessor,
	IPasswordHasher<User> passwordHasher,
	IEnumerable<IUserValidator<User>> userValidators,
	IEnumerable<IPasswordValidator<User>> passwordValidators,
	ILookupNormalizer keyNormalizer,
	IdentityErrorDescriber errors,
	IServiceProvider services,
	ILogger<UserManager<User>> logger)
	: UserManager<User>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
		errors, services, logger)
{
	public async Task<IList<UserLink>> GetUserLinksAsync(User user, CancellationToken cancellationToken = default)
	{
		user = await GetUserLinkQuery()
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		return user.UserLinks;
	}

	public async Task<IdentityResult> AddUserLinkAsync(User user, UserLink userLink,
		CancellationToken cancellationToken = default)
	{
		user = await GetUserLinkQuery()
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		if (user.UserLinks.Any(x => x.LoginProvider == userLink.LoginProvider && x.ProviderKey == userLink.ProviderKey))
		{
			return IdentityResult.Failed(ErrorDescriber.LoginAlreadyAssociated());
		}

		userLink.UserId = user.Id;
		user.UserLinks.Add(userLink);
		return await UpdateUserAsync(user).ConfigureAwait(false);
	}

	public async Task<IList<string>> GetUserApplicationIdsAsync(User user,
		CancellationToken cancellationToken = default)
	{
		user = await GetUserApplicationQuery()
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		return user.UserApplications.Select(x => x.ApplicationId).ToList();
	}

	public async Task<IdentityResult> AddUserApplicationAsync(User user, string applicationId,
		CancellationToken cancellationToken = default)
	{
		user = await GetUserApplicationQuery()
			.FirstAsync(x => x.Id == user.Id, cancellationToken);

		user.UserApplications.Add(new UserApplication
		{
			ApplicationId = applicationId,
			UserId = user.Id
		});
		return await UpdateUserAsync(user).ConfigureAwait(false);
	}


	private IQueryable<User> GetUserLinkQuery()
	{
		return userRepository.Get(new UserQueryOptions { IncludeUserLinks = true });
	}

	private IQueryable<User> GetUserApplicationQuery()
	{
		return userRepository.Get(new UserQueryOptions { IncludeUserApplications = true });
	}
}
