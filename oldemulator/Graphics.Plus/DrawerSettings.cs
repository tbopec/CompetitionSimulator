using AIRLab.Mathematics;
using AIRLab.Thornado;

namespace Eurosim.Graphics
{
	[Thornado]
	public class DrawerSettings
	{
		[Thornado]
		public Frame3D CameraLocation = new Frame3D(150, -100, 300);

		[Thornado]
		public bool FullScreen;

		[Thornado]
		public ViewModes ViewMode = ViewModes.Trackball;

		[Thornado]
		public bool ShowControls = true;

		[Thornado]
		public int RobotNumber;
	}
}