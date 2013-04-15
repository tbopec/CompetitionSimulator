using AIRLab.Thornado;
using Eurosim.ClientLib;

namespace EurosimNetworkServer
{
	public class NetworkServerHelloReply
	{
		[Thornado] public Position Position;
		[Thornado] public int SessionNumber;
	}
}