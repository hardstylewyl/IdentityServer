using IdentityServer.Domain.Entities.Abstractions;
using IdentityServer.Domain.ValueObjects;

namespace IdentityServer.Domain.Entities;

public sealed class UserLoginHistory : Entity<long>
{
	public long UserId { get; set; }

	//  登录时间
	public DateTimeOffset LoginTimeOnUtc { get; set; }

	// IP详情
	public IpAddressDetails IpInfo { get; set; }

	// 登录方式
	public string LoginMethod { get; set; }

	// 登录提供商
	public string LoginProvider { get; set; }

	// 浏览器UA标识
	public string UserAgent { get; set; }

	// 登录成功是否
	public bool LoginSuccess { get; set; }
}
