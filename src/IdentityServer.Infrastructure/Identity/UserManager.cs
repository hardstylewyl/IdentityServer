﻿using IdentityServer.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.Infrastructure.Identity;

public sealed class UserManager(
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
}
