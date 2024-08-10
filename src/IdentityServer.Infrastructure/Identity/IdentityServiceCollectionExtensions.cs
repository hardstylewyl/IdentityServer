using IdentityServer.Domain.Entites;
using IdentityServer.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Infrastructure.Identity;

public static class IdentityServiceCollectionExtensions
{
	public static IServiceCollection AddIP2RegionIpInfoService(this IServiceCollection services, string libPath)
	{
		services.AddSingleton<IIpInfoService>(_ => new IP2RegionIpInfoService(libPath));

		return services;
	}

	public static IServiceCollection AddIdentity(this IServiceCollection services)
	{
		services.AddIdentity<User, Role>()
			.AddTokenProviders()
			.AddPasswordValidators()
			.AddUserManager<UserManager>()
			.AddSignInManager<SignInManager>()
			.AddErrorDescriber<LocalizedIdentityErrorDescriber>();

		services.AddTransient<IUserStore<User>, UserStore>();
		services.AddTransient<IRoleStore<Role>, RoleStore>();
		services.AddScoped<IPasswordHasher, PasswordHasher>();

		ConfigureOptions(services);

		services.ConfigureApplicationCookie(options =>
		{
			options.LoginPath = "/Account/Login";
		});

		return services;
	}

	public static IServiceCollection AddIdentityCore(this IServiceCollection services)
	{
		services.AddIdentityCore<User>()
				.AddTokenProviders()
				.AddPasswordValidators()
				.AddUserManager<UserManager>()
				.AddSignInManager<SignInManager>()
				.AddErrorDescriber<LocalizedIdentityErrorDescriber>();

		services.AddTransient<IUserStore<User>, UserStore>();
		services.AddTransient<IRoleStore<Role>, RoleStore>();
		services.AddScoped<IPasswordHasher, PasswordHasher>();

		ConfigureOptions(services);

		return services;
	}

	private static IdentityBuilder AddTokenProviders(this IdentityBuilder identityBuilder)
	{
		identityBuilder
			.AddDefaultTokenProviders()
			.AddTokenProvider<AuthenticatorTokenProvider<User>>(TokenOptions.DefaultAuthenticatorProvider)
			.AddTokenProvider<EmailConfirmationTokenProvider<User>>("EmailConfirmation");

		return identityBuilder;
	}

	private static IdentityBuilder AddPasswordValidators(this IdentityBuilder identityBuilder)
	{
		//identityBuilder
		//	.AddPasswordValidator<WeakPasswordValidator>()
		//	.AddPasswordValidator<HistoricalPasswordValidator>();

		return identityBuilder;
	}

	private static void ConfigureOptions(IServiceCollection services)
	{
		services.Configure<IdentityOptions>(o =>
		{
			//里面配置一些规则，密码强度/生成密码的规则
			o.Password.RequireDigit = false; //是否必须有数字
			o.Password.RequireLowercase = false; //是否必须有小写字母
			o.Password.RequireNonAlphanumeric = false; //是否必须有除字母数字之外的其他字符
			o.Password.RequireUppercase = false; //是否必须有大写字母
			o.Password.RequiredLength = 6; //最短限制
			o.Lockout.MaxFailedAccessAttempts = 3; //登录失败超过3次触发锁定
			o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //配置锁定时间 5分钟

			//如果配置生成为验证码（较短），若为链接则不用配置此项（生成token较长）
			//获取/配置用于重置密码的令牌为 Email
			o.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
			//获取/配置电子邮件确认令牌的提供程序（邮件激活）
			o.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
			//设置短token
			o.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
		});

		services.Configure<DataProtectionTokenProviderOptions>(options =>
		{
			options.TokenLifespan = TimeSpan.FromHours(3);
		});

		services.Configure<EmailConfirmationTokenProviderOptions>(options =>
		{
			options.TokenLifespan = TimeSpan.FromDays(2);
		});

		services.Configure<PasswordHasherOptions>(option =>
		{
			// option.IterationCount = 10000;
			//option.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
		});
	}
}
