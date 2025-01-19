using System.Xml.Serialization;
using Xunit;

namespace DigitalProduction.UnitTests;

/// <summary>
/// A generic company.
/// </summary>
[XmlRoot("company")]
public class Company
{
	#region Construction

	/// <summary>
	/// Default constructor.  Required for serialization.
	/// </summary>
	public Company()
	{
	}

	#endregion

	#region Properties

	/// <summary>
	/// Name.
	/// </summary>
	[XmlAttribute("name")]
	public string Name { get;  set; } = "";

	/// <summary>
	/// Number of people in the family.
	/// </summary>
	[XmlIgnore()]
	public int NumberOfEmployees { get => Employees.Count; }

	/// <summary>
	/// Employees.
	/// </summary>
	[XmlArray("employees"), XmlArrayItem("employee")]
	public List<Person> Employees { get; set; } = new();

	/// <summary>
	/// Assets.
	/// </summary>
	[XmlArray("assets"), XmlArrayItem("asset")]
	public List<Asset> Assets { get; set; } = new();

	#endregion

	#region Methods

	/// <summary>
	/// Find a person in the family by name.
	/// </summary>
	/// <param name="name">Name of the Person to find.</param>
	/// <returns>The first Person in the list with the specified name.</returns>
	public Person? GetEmployee(string name)
	{
		return Employees.Find(x => x.Name == name);
	}

	/// <summary>
	/// Find a person in the family by name.
	/// </summary>
	/// <param name="name">Name of the Person to find.</param>
	/// <returns>The first Person in the list with the specified name.</returns>
	public Asset? GetAsset(string name)
	{
		return Assets.Find(x => x.Name == name);
	}
	
	#endregion

} // End class.