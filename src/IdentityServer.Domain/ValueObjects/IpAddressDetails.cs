namespace IdentityServer.Domain.ValueObjects;

public class IpAddressDetails : ValueObject
{
	public static IpAddressDetails None = new() { Ip = "未知" };
	public static IpAddressDetails Local = new() { Ip = "本机地址" };
	public string Ip { get; private set; }
	public string Street { get; private set; }
	public string City { get; private set; }
	public string State { get; private set; }
	public string Country { get; private set; }
	public string ZipCode { get; private set; }

	public IpAddressDetails() { }

	public IpAddressDetails(string ip, string street, string city, string state, string country, string zipcode)
	{
		Ip = ip;
		Street = street;
		City = city;
		State = state;
		Country = country;
		ZipCode = zipcode;
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Ip;
		yield return Street;
		yield return City;
		yield return State;
		yield return Country;
		yield return ZipCode;
	}
}
