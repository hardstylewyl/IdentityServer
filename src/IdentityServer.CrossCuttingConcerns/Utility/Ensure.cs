namespace IdentityServer.CrossCuttingConcerns.Utility;

public static class Ensure
{
	public static void NotEmpty(string? value, string message, string argumentName)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new ArgumentException(message, argumentName);
		}
	}

	public static void NotEmpty(Guid value, string message, string argumentName)
	{
		if (value == Guid.Empty)
		{
			throw new ArgumentException(message, argumentName);
		}
	}

	public static void NotEmpty(long? value, string message, string argumentName)
	{
		if (value is null or 0)
		{
			throw new ArgumentException(message, argumentName);
		}
	}

	public static void NotEmpty(DateTime value, string message, string argumentName)
	{
		if (value == default)
		{
			throw new ArgumentException(message, argumentName);
		}
	}

	public static void NotNull<T>(T? value, string message, string argumentName)
		where T : class
	{
		if (value is null)
		{
			throw new ArgumentNullException(argumentName, message);
		}
	}

	public static void True(bool value, string message, string argumentName)
	{
		if (!value)
		{
			throw new ArgumentException(message, argumentName);
		}
	}

	public static void ContainedIn<T>(T value, IEnumerable<T> values, string message, string argumentName)
	{
		if (!values.Contains(value))
		{
			throw new ArgumentException(message, argumentName);
		}
	}
}
