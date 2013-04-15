using System;
using AIRLab.Thornado;

namespace Eurosim.Core
{
	[Serializable]
	public class RobotCameraData
	{
		[Thornado]
		public byte[] Bitmap;
	}
}