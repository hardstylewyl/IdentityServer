using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace IdentityServer.Mvc.Models.Common;

public enum AlertType
{
	Toast,
	Success,
	Warning,
	Error
}

public sealed class AlertModel
{
	public const string Key = nameof(AlertModel);

	public static readonly AlertModel None = new();

	public AlertType Type { get; set; } = AlertType.Success;
	public string Message { get; set; } = string.Empty;
	public uint Duration { get; set; } = 2000;

	public static AlertModel Toast(string message, uint duration = 2000) =>
		new() { Message = message, Type = AlertType.Toast, Duration = duration };

	public static AlertModel Success(string message, uint duration = 2000) =>
		new() { Message = message, Type = AlertType.Success, Duration = duration };

	public static AlertModel Warning(string message, uint duration = 2000) =>
		new() { Message = message, Type = AlertType.Warning, Duration = duration };

	public static AlertModel Error(string message, uint duration = 2000) =>
		new() { Message = message, Type = AlertType.Error, Duration = duration };

	public void WriteTempData(ITempDataDictionary tempData)
	{
		tempData[Key] = JsonSerializer.Serialize(this);
	}

	public static bool TryGet(ITempDataDictionary tempData,out AlertModel? alert)
	{
		var success = tempData.TryGetValue(Key, out object? value);

		alert = success && value is string json
			? alert = JsonSerializer.Deserialize<AlertModel>(json)
			: alert = null;

		return success;
	}
}
