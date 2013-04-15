using System;
using AIRLab.Thornado;
using RoboCoP.Plus.Common;

namespace Eurosim.Core
{
	[Serializable]
	public class DoubleWheelMovementSettings : ConfirmingServiceSettings
	{
		// настройки шума и расстояние между колесами

		public double TransformVelocity(double v, bool left)
		{
			if(left)
				v *= LeftSecondDegree * v + LeftFirstDegree + 1;
			else
				v *= RightSecondDegree * v + RightFirstDegree + 1;
			v += (Random.NextDouble() - 0.5) * VelocityWhiteNoise;
			return v;
		}

		[Thornado]
		public double VelocityWhiteNoise { get; set; }

		[Thornado]
		public double LeftSecondDegree { get; set; }

		[Thornado]
		public double RightSecondDegree { get; set; }

		[Thornado]
		public double LeftFirstDegree { get; set; }

		[Thornado]
		public double RightFirstDegree { get; set; }

		[Thornado]
		public double DistanceWheels = 10;

		private static readonly Random Random = new Random();
	}
}