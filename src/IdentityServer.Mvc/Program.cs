using System.Reflection;
using IdentityServer.Domain.Notification;
using IdentityServer.Infrastructure.DateTimes;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Infrastructure.Localization;
using IdentityServer.Mvc.ConfigurationOptions;
using IdentityServer.Mvc.Services;
using IdentityServer.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Quartz;
using Scopes = OpenIddict.Abstractions.OpenIddictConstants.Scopes;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);

services.AddControllersWithViews();
services.AddDateTimeProvider();
services.AddPersistence("Server=127.0.0.1;Port=5432;Database=identityserver;Uid=postgres;Pwd=wyl123567;",
	Assembly.GetExecutingAssembly().GetName().Name!);
services.AddIdentityServerLocalization();
services.AddIdentity();

#region Quartz

//OpenIddict提供与Quartz.NET的本机集成，以执行计划任务
//（如从数据库中删除孤立的授权/令牌）。
services.AddQuartz(o =>
	{
		o.UseSimpleTypeLoader();
		o.UseInMemoryStore();
	})
	//注册Quartz.NET服务并将其配置为在作业完成之前阻止关闭。
	.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

#endregion

#region OpenIddict

services.AddOpenIddict()
	.AddCore(o =>
	{
		o.UseEntityFrameworkCore()
			.UseDbContext<IdentityServerDbContext>();

		o.UseQuartz();
	})
	.AddServer(o =>
	{
		//****加密key and 签名key****
		//在现实世界的应用程序中，此加密密钥应存储在安全的地方（例如，在Azure KeyVault中，作为机密存储）。
		//var encryptionKey = new SymmetricSecurityKey(Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY="));
		//var signingnKey = new SymmetricSecurityKey(Convert.FromBase64String("DWgd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY="));

		//配置对称加密的密钥，不如证书安全性高
		//需要在客户端/资源端 以及授权服务端同时使用相同的key
		// o.AddEncryptionKey(encryptionKey)
		// 	.AddSigningKey(signingnKey);

		//TODO:开发环境 注册签名和证书
		o.AddDevelopmentEncryptionCertificate()
			.AddDevelopmentSigningCertificate();

		//****生产环境需要使用证书****
		//  my - signing - certificate.pfx 和 my-password 分别表示签名证书文件名和密码
		//  my - encryption - certificate.pem 表示加密证书文件名
		//  o.AddSigningCertificate(Assembly.GetExecutingAssembly(), "my-signing-certificate.pfx", "my-password")
		//.AddEncryptionCertificate("my-encryption-certificate.pem");
		// o.AddEncryptionCertificate(identityServerOptions.EncryptionCertificate.FindCertificate())
		// 	.AddSigningCertificate(identityServerOptions.SigningCertificate.FindCertificate());

		//支持客户端/密码/设备码/授权码/刷新令牌授权类型
		o.AllowClientCredentialsFlow()
			.AllowPasswordFlow()
			.AllowDeviceCodeFlow()
			.AllowAuthorizationCodeFlow()
			.AllowRefreshTokenFlow();

		//PKCE只在客户端无法保护自己的client_secret的时候使用
		o.RequireProofKeyForCodeExchange();

		//支持的作用域,注意：如果在此处不指定，那么就算client中有此scope也会不允许请求
		o.RegisterScopes(Scopes.Profile, Scopes.Email, Scopes.Roles);

		//配置传递模式
		o.UseAspNetCore()
			.EnableAuthorizationEndpointPassthrough() //授权
			.EnableLogoutEndpointPassthrough() //登出
			.EnableTokenEndpointPassthrough() //获取token
			.EnableUserinfoEndpointPassthrough() //获取用户信息
			.EnableVerificationEndpointPassthrough()//启用验证签名有效性
			.EnableStatusCodePagesIntegration(); //启用状态码页面集成支持

		//注册oauth2.x的功能端点
		o.SetAuthorizationEndpointUris("connect/authorize") //授权
			.SetLogoutEndpointUris("connect/logout") //登出
			.SetTokenEndpointUris("connect/token") //获取token
			.SetUserinfoEndpointUris("connect/userinfo") //获取用户信息 
			.SetRevocationEndpointUris("connect/revoke") //撤销token   
			.SetIntrospectionEndpointUris("connect/introspect") //内省，验证令牌有效性
			.SetVerificationEndpointUris("connect/verify") //验证签名有效性
			.SetDeviceEndpointUris("connect/device");//设备授权

		//配置过期时间
		o.SetIdentityTokenLifetime(TimeSpan.FromDays(3)) //配置id_token过期时间
			.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(3)) //配置authorization_code过期时间
			.SetAccessTokenLifetime(TimeSpan.FromDays(3)) //配置access_token过期时间
			.SetRefreshTokenLifetime(TimeSpan.FromDays(1)) //配置refresh_token过期时间
			.SetDeviceCodeLifetime(TimeSpan.FromMinutes(3)); //配置device_code过期时间
	})
	.AddValidation(o =>
	{
		o.UseLocalServer();
		o.UseAspNetCore();
	});

#endregion

#region Authentication And Authorization

services.AddAuthorization();
services.AddAuthentication(o =>
	{
		o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		o.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
		o.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
		o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddGitee(o =>
	{
		o.ClientId = appSettings.Gitee.ClientId;
		o.ClientSecret = appSettings.Gitee.ClientSecret;
		o.CallbackPath = "/callback_login/gitee";
		o.CorrelationCookie.SameSite = SameSiteMode.Lax;
		o.ClaimActions.MapCustomJson("all", element => element.ToString());
	})
	.AddGitHub(o =>
	{
		o.ClientId = appSettings.GitHub.ClientId;
		o.ClientSecret = appSettings.GitHub.ClientSecret;
		o.CallbackPath = "/callback_login/github";
		o.CorrelationCookie.SameSite = SameSiteMode.Lax;
		o.ClaimActions.MapCustomJson("all", element => element.ToString());
	});

#endregion

#region Fake

services.AddScoped<ISmsNotification, FakeNoticeService>();
services.AddScoped<IEmailNotification, FakeNoticeService>();

#endregion Fake

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();
