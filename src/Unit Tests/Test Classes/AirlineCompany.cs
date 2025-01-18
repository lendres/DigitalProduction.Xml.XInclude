using System.Xml.Serialization;
using System.ComponentModel;

namespace DigitalProduction.UnitTests;

/// <summary>
/// An airline company.
/// </summary>
[XmlRoot("airline")]
[DisplayName("Airline")]
[Description("A company that owns and operates airplanes.")]
public class AirlineCompany : Company
{
	#region Construction

	/// <summary>
	/// Default constructor.  Required for serialization.
	/// </summary>
	public AirlineCompany()
	{
	}

	#endregion

	#region Properties

	/// <summary>
	/// Number of planes the airline has available.
	/// </summary>
	[XmlAttribute("numberofplanes")]
	public int NumberOfPlanes { get; set; } = 0;

	#endregion

	#region Static Functions

	/// <summary>
	/// Helper function to create an airline.
	/// </summary>
	/// <returns>A new airline populated with some default values.</returns>
	public static AirlineCompany CreateAirline()
	{
		AirlineCompany company = new()
		{
			Name            = "Oceanic",
			NumberOfPlanes  = 10
		};
		company.Employees.Add(new Person("Manager", 36, Gender.Female));
		company.Employees.Add(new Person("Luggage Handler", 37, Gender.Male));
		company.Employees.Add(new Person("Pilot", 28, Gender.Female));
		company.Employees.Add(new Person("Captain", 30, Gender.Male));

		company.Assets.Add(new Asset("Embraer Phenom 300E", 10000000, "Long-range single-pilot jet, with a maximum range of 2,010 nautical miles. It seats six to 10 passengers."));
		company.Assets.Add(new Asset("Cessna Citation CJ4 Gen2", 9000000, "Single-pilot plane that seats up to 10 passengers."));
		company.Assets.Add(new Asset("HondaJet HA-420", 8000000, "One of the smallest and lightest jets around with enhanced efficiency."));
		company.Assets.Add(new Asset("Cirrus Vision Jet", 7000000, " Includes a Cirrus Airframe Parachute System (CAPS)."));

		return company;
	}

	#endregion

} // End class.