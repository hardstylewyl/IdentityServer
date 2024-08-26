using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace IdentityServer.Mvc.Models.Common;

public sealed class ExternalProfileModel
{
	// 第三方标识id
	[JsonPropertyName("id")] public int Id { get; set; }

	// 登录名
	[JsonPropertyName("login")] public string Login { get; set; }

	// 姓名
	[JsonPropertyName("name")] public string Name { get; set; }

	// 头像地址
	[JsonPropertyName("avatar_url")] public string Avatar { get; set; }

	// 邮箱
	[JsonPropertyName("email")] public string Email { get; set; }

	public ExternalLoginInfo ExternalLoginInfo { get; set; }

	public static ExternalProfileModel? CreateForExternalLoginInfo(ExternalLoginInfo loginInfo)
	{
		//获取第三方返回的所以信息的json
		var json = loginInfo.Principal.FindFirstValue("all");
		
		//解析
		var model = string.IsNullOrEmpty(json) 
			? null 
			: JsonSerializer.Deserialize<ExternalProfileModel>(json);
		
		if (model is null) return null;
		
		model.ExternalLoginInfo = loginInfo;
		return model;
	}
	
	
	public UserLink CreateUserLink() =>
		new()
		{
			LoginProvider = ExternalLoginInfo.LoginProvider,
			ProviderKey = ExternalLoginInfo.ProviderKey,
			ProviderDisplayName =ExternalLoginInfo.ProviderDisplayName,
			LoginName = Login,
			UserId = 0
		};

	
	public IEnumerable<Claim> ExtractClaims() =>
	[
		new Claim(OpenIddictConstants.Claims.Gender, ""),
		new Claim(OpenIddictConstants.Claims.Address, ""),
		new Claim(OpenIddictConstants.Claims.Birthdate, ""),
		new Claim(OpenIddictConstants.Claims.Picture,
			string.IsNullOrWhiteSpace(Avatar)
				? $"https://ui-avatars.com/api/?name={Name}&background=0D8ABC"
				: Avatar),

		new Claim(OpenIddictConstants.Claims.Name, Name),
		new Claim(OpenIddictConstants.Claims.Nickname, Name)
	];
}
