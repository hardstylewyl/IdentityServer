namespace IdentityServer.Domain.ValueObjects;

public class IpAddressDetails : ValueObject
{
	public static IpAddressDetails None = new() { Ip = "未知" };
	public static IpAddressDetails Local = new() { Ip = "本机地址" };
	public string Ip { get; private set; }

	//市区
	public string City { get; private set; }
	//省级
	public string State { get; private set; }
	//国家
	public string Country { get; private set; }
	//运营商
	public string Carrier { get; set; }
	//街道
	public string Street { get; private set; }
	//邮编
	public string ZipCode { get; private set; }


	public IpAddressDetails() { }

	public IpAddressDetails(string ip, string city, string state, string country, string carrier, string street, string zipCode)
	{
		Ip = ip;
		City = city;
		State = state;
		Country = country;
		Carrier = carrier;
		Street = street;
		ZipCode = zipCode;
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return Ip;
		yield return Street;
		yield return City;
		yield return State;
		yield return Country;
		yield return ZipCode;
		yield return Carrier;
	}
}
