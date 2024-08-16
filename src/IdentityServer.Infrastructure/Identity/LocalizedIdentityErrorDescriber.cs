using IdentityServer.Infrastructure.Identity.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace IdentityServer.Infrastructure.Identity;

public class LocalizedIdentityErrorDescriber(IStringLocalizer<IdentityLocalization> localizer) : IdentityErrorDescriber
{
	public override IdentityError DefaultError() => new()
	{
		Code = nameof(DefaultError),
		Description = localizer[nameof(DefaultError)]
	};

	public override IdentityError ConcurrencyFailure() => new()
	{
		Code = nameof(ConcurrencyFailure),
		Description = localizer[nameof(ConcurrencyFailure)]
	};

	public override IdentityError PasswordMismatch() => new()
	{
		Code = nameof(PasswordMismatch),
		Description = localizer[nameof(PasswordMismatch)]
	};

	public override IdentityError InvalidToken() => new()
	{
		Code = nameof(InvalidToken),
		Description = localizer[nameof(InvalidToken)]
	};

	public override IdentityError RecoveryCodeRedemptionFailed() => new()
	{
		Code = nameof(RecoveryCodeRedemptionFailed),
		Description = localizer[nameof(RecoveryCodeRedemptionFailed)]
	};

	public override IdentityError LoginAlreadyAssociated() => new()
	{
		Code = nameof(LoginAlreadyAssociated),
		Description = localizer[nameof(LoginAlreadyAssociated)]
	};
	
	public override IdentityError InvalidUserName(string? userName) => new()
	{
		Code = nameof(InvalidUserName),
		Description = string.Format(localizer[nameof(InvalidUserName)], userName)
	};

	public override IdentityError InvalidEmail(string? email) => new()
	{
		Code = nameof(InvalidEmail),
		Description = string.Format(localizer[nameof(InvalidEmail)], email)
	};

	public override IdentityError DuplicateUserName(string userName) => new()
	{
		Code = nameof(DuplicateUserName),
		Description = string.Format(localizer[nameof(DuplicateUserName)], userName)
	};

	public override IdentityError DuplicateEmail(string email) => new()
	{
		Code = nameof(DuplicateEmail),
		Description = string.Format(localizer[nameof(DuplicateEmail)], email)
	};

	public override IdentityError InvalidRoleName(string? role) => new()
	{
		Code = nameof(InvalidRoleName),
		Description = string.Format(nameof(InvalidRoleName), role)
	};

	public override IdentityError DuplicateRoleName(string role) => new()
	{
		Code = nameof(DuplicateRoleName),
		Description = string.Format(localizer[nameof(DuplicateRoleName)], role)
	};

	public override IdentityError UserAlreadyHasPassword() => new()
	{
		Code = nameof(UserAlreadyHasPassword),
		Description = localizer[nameof(UserAlreadyHasPassword)]
	};

	public override IdentityError UserLockoutNotEnabled() => new()
	{
		Code = nameof(UserLockoutNotEnabled),
		Description = localizer[nameof(UserLockoutNotEnabled)]
	};

	public override IdentityError UserAlreadyInRole(string role) => new()
	{
		Code = nameof(UserAlreadyInRole),
		Description = string.Format(localizer[nameof(UserAlreadyInRole)], role)
	};

	public override IdentityError UserNotInRole(string role) => new()
	{
		Code = nameof(UserNotInRole),
		Description = string.Format(localizer[nameof(UserNotInRole)], role)
	};

	public override IdentityError PasswordTooShort(int length) => new()
	{
		Code = nameof(PasswordTooShort),
		Description = string.Format(localizer[nameof(PasswordTooShort)], length)
	};

	public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
	{
		Code = nameof(PasswordRequiresUniqueChars),
		Description = string.Format(localizer[nameof(PasswordRequiresUniqueChars)], uniqueChars)
	};

	public override IdentityError PasswordRequiresNonAlphanumeric() => new()
	{
		Code = nameof(PasswordRequiresNonAlphanumeric),
		Description = localizer[nameof(PasswordRequiresNonAlphanumeric)]
	};

	public override IdentityError PasswordRequiresDigit() => new()
	{
		Code = nameof(PasswordRequiresDigit),
		Description = localizer[nameof(PasswordRequiresDigit)]
	};

	public override IdentityError PasswordRequiresLower() => new()
	{
		Code = nameof(PasswordRequiresLower),
		Description = localizer[nameof(PasswordRequiresLower)]
	};

	public override IdentityError PasswordRequiresUpper() => new()
	{
		Code = nameof(PasswordRequiresUpper),
		Description = localizer[nameof(PasswordRequiresUpper)]
	};
}
