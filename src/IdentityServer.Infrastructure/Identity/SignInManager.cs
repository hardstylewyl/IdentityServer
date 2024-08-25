using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Identity;
using IdentityServer.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.Infrastructure.Identity;

public sealed class SignInManager(
	IIpInfoService ipInfoService,
	IUserRepository userRepository,
	UserManager<User> userManager,
	IHttpContextAccessor contextAccessor,
	IUserClaimsPrincipalFactory<User> claimsFactory,
	IOptions<IdentityOptions> optionsAccessor,
	ILogger<SignInManager<User>> logger,
	IAuthenticationSchemeProvider schemes,
	IUserConfirmation<User> confirmation)
	: SignInManager<User>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
	private readonly UserManager<User> _userManager = userManager;
	
	public async Task<IList<UserLoginHistory>> GetLoginHistoriesAsync(User user,
		CancellationToken cancellationToken = default)
	{
		user =  await userRepository.Get(new UserQueryOptions{IncludeUserLoginHistories = true})
			.FirstAsync(x=>x.Id==user.Id, cancellationToken);
		
		return user.UserLoginHistories;
	}
	
	public async Task<IdentityResult> AddLoginHistoryAsync(User user,
		string loginMethod,
		string loginProvider = "LocalProvider", 
		bool loginSuccess = true,
		CancellationToken cancellationToken = default)
	{
		user = await userRepository.Get(new UserQueryOptions{IncludeUserLoginHistories = true})
			.FirstAsync(x=>x.Id==user.Id, cancellationToken);
		
		//获取ip信息和UserAgent信息
		var ip = Context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
		var ua = Context.Request.Headers.UserAgent.ToString();
		
		//如果最近一次登录记录在1天并且ip相同，则不记录
		if (user.UserLoginHistories.Count > 0)
		{
			var lastLoginHistory = user.UserLoginHistories.MaxBy(x=>x.LoginTimeOnUtc)!;
			//如果最近一次登录记录在1天，则不记录
			if (DateTimeOffset.UtcNow - lastLoginHistory.LoginTimeOnUtc < TimeSpan.FromDays(1) 
			    && lastLoginHistory.IpInfo.Ip.Equals(ip))
			{
				return IdentityResult.Success;
			}
		}
		
		var ipInfo = await ipInfoService.GetIpInfoAsync(ip,cancellationToken);
		var userLoginHistory = new UserLoginHistory()
		{
			UserId = user.Id,
			LoginTimeOnUtc =  DateTimeOffset.UtcNow,
			UserAgent = ua,
			IpInfo = ipInfo,
			LoginProvider = loginProvider,
			LoginMethod = loginMethod,
			LoginSuccess = loginSuccess
		};
		
		user.UserLoginHistories.Add(userLoginHistory);
		return await _userManager.UpdateAsync(user).ConfigureAwait(false);
	}
}
