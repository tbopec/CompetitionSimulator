using System;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using RoboCoP.Common;

namespace Eurosim.Core
{
	public enum NoiseModel
	{
		Local,
		Integral
	}

	[Serializable]
	public class EmulatedNavigatorSettings : SensorSettings
	{
		[Thornado]
		public Angle AngleNoise { get; set; }

		[Thornado]
		public double LengthNoise { get; set; }

		[Thornado]
		public double TimeNoise { get; set; }

		public bool IsPlane { get { return true; } }

		[Thornado]
		public NoiseModel NoiseModel;
	}
}