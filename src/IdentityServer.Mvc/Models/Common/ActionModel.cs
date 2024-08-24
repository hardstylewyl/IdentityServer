using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace IdentityServer.Mvc.Models.Common;

public enum ActionType
{
	Redirect,
}

public sealed class ActionModel
{
	public const string Key = nameof(ActionModel);
	
	public static readonly ActionModel None = new();

	public ActionType Type { get; set; } = ActionType.Redirect;
	public string Url { get; set; } = string.Empty;
	public uint Delay { get; set; } = 1500;

	public static ActionModel Redirect(string url, uint delay = 1500)
		=> new() { Type = ActionType.Redirect, Url = url, Delay = delay };
	
	public void WriteTempData(ITempDataDictionary tempData)
	{
		tempData[Key] = JsonSerializer.Serialize(this);
	}

	public static bool TryGet(ITempDataDictionary tempData,out ActionModel? alert)
	{
		var success = tempData.TryGetValue(Key, out object? value);

		alert = success && value is string json
			? alert = JsonSerializer.Deserialize<ActionModel>(json)
			: alert = null;

		return success;
	}
}
