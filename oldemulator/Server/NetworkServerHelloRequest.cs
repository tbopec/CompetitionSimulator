using AIRLab.Thornado;
using Eurosim.ClientLib;

namespace EurosimNetworkServer
{
	public class NetworkServerHelloRequest
	{
		[Thornado] public string Affiliation;
		[Thornado] public string City;
		[Thornado] public string Country;
		[Thornado] public string Email;
		[Thornado] public int Field;
		[Thornado] public string Name;
		[Thornado] public Opponents Opponent;
		[Thornado] public Position Position;
	}
}