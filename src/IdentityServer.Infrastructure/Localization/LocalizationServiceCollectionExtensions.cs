using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Infrastructure.Localization;

public static class LocalizationServiceCollectionExtensions
{
	public static IServiceCollection AddIdentityServerLocalization(this IServiceCollection services, LocalizationProviders? providers = null)
	{
		//默认的支持
		//services.AddScoped<IStringLocalizer, DefaultStringLocalizer>();

		services.AddLocalization();

		services.Configure<RequestLocalizationOptions>(options =>
		{
			var cultureInfos = new List<CultureInfo>
			{
				new("zh-CN"),
				new("en-US"),
				new("uk-UA"),
			};

			options.DefaultRequestCulture = new RequestCulture("zh-CN", "zh-CN");
			options.SupportedCultures = cultureInfos;
			options.SupportedUICultures = cultureInfos;
			options.ApplyCurrentCultureToResponseHeaders = true;

			options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
			{
				return new ProviderCultureResult("zh-CN");
			}));
		});

		return services;
	}
}
