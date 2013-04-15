using System;
using AIRLab.Thornado;
using RoboCoP.Common;

namespace Eurosim.Core
{
	[Serializable]
	public class MagicEyeSettings : SensorSettings
	{
		[Thornado]
		public double Radius = 1000;

		[Thornado]
		public bool LocalCoordinates = true;
	}
}