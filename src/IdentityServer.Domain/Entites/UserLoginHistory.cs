using IdentityServer.Domain.Entites.Abstractions;
using IdentityServer.Domain.ValueObjects;

namespace IdentityServer.Domain.Entites;

public class UserLoginHistory : Entity<long>
{
	public long UserId { get; set; }

	/// <summary>
	///  登录时间
	/// </summary>
	public DateTimeOffset LoginTimeOnUtc { get; set; }

	/// <summary>
	/// IP详情
	/// </summary>
	public IpAddressDetails IpInfo { get; set; }

	/// <summary>
	/// 登录方式
	/// </summary>
	public string LoginMethod { get; set; }

	/// <summary>
	/// 登录提供商
	/// </summary>
	public string LoginProvider { get; set; }

	/// <summary>
	/// 浏览器UA标识
	/// </summary>
	public string UserAgent { get; set; }

	/// <summary>
	/// 登录成功是否
	/// </summary>
	public bool LoginSuccess { get; set; }
}
