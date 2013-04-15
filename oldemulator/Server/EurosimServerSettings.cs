using AIRLab.Thornado;
using Eurosim.Core;

namespace EurosimNetworkServer
{
	[Thornado]
	public class EurosimServerSettings
	{
		[Thornado]
		public int Port;

		[Thornado]
		public EmulatorSettings Emulator;

		[Thornado]
		public RobotSettings PlayerRobot;
	}
}