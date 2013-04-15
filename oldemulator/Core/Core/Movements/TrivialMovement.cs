using System;
using AIRLab.Mathematics;
using AIRLab.Thornado;

namespace Eurosim.Core
{
	[Serializable]
	public class TrivialPlaneMovement : IPlaneMovement
	{
		[Thornado]
		public double TotalTime { get; set; }

		public Frame2D GetOffset(double startTime, double dtime)
		{
			if(TotalTime == 0) return new Frame2D();
			Angle startAngle = Offset.Angle * startTime / TotalTime;
			double cos = Math.Cos(startAngle.Radian);
			double sin = Math.Sin(startAngle.Radian);
			double dx = (Offset.X * cos + Offset.Y * sin) * dtime / TotalTime;
			double dy = (-Offset.X * sin + Offset.Y * cos) * dtime / TotalTime;
			Angle da = Offset.Angle * dtime / TotalTime;
			return new Frame2D(dx, dy, da);
		}

		[Thornado]
		public Frame2D Offset;
	}
}