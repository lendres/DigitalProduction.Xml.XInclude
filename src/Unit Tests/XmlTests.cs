using DigitalProduction.Xml.XInclude;
using System.Xml.Serialization;

namespace DigitalProduction.UnitTests;

public class XmlTests
{
	/// <summary>
	/// Basic serialization and deserialization test.
	/// </summary>
	[Fact]
	public void XmlDeSerialization()
	{

		string file = ".\\Test Files\\airline.xml";

		AirlineCompany? airline = null;

		try
		{
			XmlSerializer serializer				= new(typeof(AirlineCompany));
			XIncludingReader xmlIncludingReader		= new(file);
			airline									= (AirlineCompany?)serializer.Deserialize(xmlIncludingReader);
			xmlIncludingReader.Close();
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine("\n\n"+exception.Message+"\n\n");
		}

		Assert.NotNull(airline);
		Assert.Equal(10, airline.NumberOfPlanes);

		Person? person = airline.GetEmployee("Pilot");
		Assert.NotNull(person);
		Assert.Equal(28, person.Age);

		Asset? asset = airline.GetAsset("Embraer Phenom 300E");
		Assert.NotNull(asset);
		Assert.Equal(10000000, asset.Value);
	}

} // End class.