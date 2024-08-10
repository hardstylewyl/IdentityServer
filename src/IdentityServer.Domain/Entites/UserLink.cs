﻿using IdentityServer.Domain.Entites.Abstractions;

namespace IdentityServer.Domain.Entites;

public class UserLink : Entity<long>
{
	/// <summary>
	/// Gets or sets the login provider for the login (e.g. facebook, google)
	/// </summary>
	public string LoginProvider { get; set; }

	/// <summary>
	/// Gets or sets the unique provider identifier for this login.
	/// </summary>
	public string ProviderKey { get; set; }

	/// <summary>
	/// Gets or sets the friendly name used in a UI for this login.
	/// </summary>
	public string? ProviderDisplayName { get; set; }

	/// <summary>
	/// Gets or sets the primary key of the user associated with this login.
	/// </summary>
	public long UserId { get; set; }

	/// <summary>
	/// 建立关联的时间
	/// </summary>
	public DateTimeOffset LinkTimeOnUtc { get; set; } = DateTimeOffset.UtcNow;

	/// <summary>
	/// 登录名
	/// </summary>
	public string LoginName { get; set; }
}
